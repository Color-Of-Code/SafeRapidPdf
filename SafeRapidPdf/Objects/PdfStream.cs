using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using SafeRapidPdf.Parsing;

namespace SafeRapidPdf.Objects
{
    public sealed class PdfStream : PdfObject
    {
        private PdfStream(PdfDictionary dictionary, PdfData data)
            : base(PdfObjectType.Stream)
        {
            IsContainer = true;
            StreamDictionary = dictionary;
            Data = data;
        }

        public PdfDictionary StreamDictionary { get; }

        public PdfData Data { get; }

        public override IReadOnlyList<IPdfObject> Items
        {
            get
            {
                var list = new List<IPdfObject>(StreamDictionary.Items.Count + 1);
                list.AddRange(StreamDictionary.Items);
                list.Add(Data);
                return list;
            }
        }

        private byte[] FlateDecodeWithPredictorNone(int _, byte[] decompressed)
        {
            return decompressed;
        }

        private byte[] FlateDecodeWithPredictorPngUp(int columns, byte[] decompressed)
        {
            var output = new List<byte>(32 * 1024);
            var previousRow = new byte[columns];
            for (int i = 0; i < columns; i++)
                previousRow[i] = 0;
            int rows = decompressed.Length / (columns + 1); // we have an additional predictor byte in the source
            for (int r = 0; r < rows; r++)
            {
                var currentRow = new byte[columns];
                byte rowPredictor = decompressed[r * (columns + 1)];
                if (rowPredictor != 2)
                {
                    throw new NotImplementedException("Only up predictor is supported at the moment");
                }
                for (int i = 0; i < columns; i++)
                {
                    // the leading predictor is ignored, assuming it's always UP
                    var inputByte = decompressed[(r * (columns + 1)) + i + 1];
                    currentRow[i] = (byte)(inputByte + previousRow[i]);
                    output.Add(currentRow[i]);
                }
                previousRow = currentRow;
            }
            return output.ToArray();
        }

        private byte[] FlateDecodeWithPredictor(int predictor, int columns, byte[] input)
        {
            // now we have to handle the predictors...
            return predictor switch
            {
                //1 = default: no prediction
                1 => FlateDecodeWithPredictorNone(columns, input),
                //12 = PNG prediction (on encoding, PNG Up on all rows)
                12 => FlateDecodeWithPredictorPngUp(columns, input),
                _ => throw new NotImplementedException($"Sorry at the moment predictor {predictor} is not implemented. Please make a feature request on https://github.com/jdehaan/SafeRapidPdf/issues. Ideally provide an example pdf."),
            };
        }

        public byte[] Decode()
        {
            if (!StreamDictionary.TryGetValue("Filter", out IPdfObject filter))
            {
                // filter is optional
                // no filter provided= return the data as-is
                return Data.Data;
            }

            // TODO: multiple filter in order can be specified
            if (filter.Text == "FlateDecode")
            {

                var data = new MemoryStream(Data.Data);

                // Read the ZLIB header
                _ = data.ReadByte(); // 104
                _ = data.ReadByte(); // 222

                byte[] decompressed;

                using (var output = new MemoryStream())
                using (var deflatedStream = new DeflateStream(data, CompressionMode.Decompress))
                {
                    deflatedStream.CopyTo(output);

                    decompressed = output.ToArray();
                }

                // set defaults
                int predictor = 1; // no prediction
                int columns = 1;

                if (StreamDictionary.TryGetValue("DecodeParms", out var decodeParams))
                {
                    var parameters = (PdfDictionary)decodeParams;
                    columns = ((PdfNumeric)parameters["Columns"]).ToInt32();
                    predictor = ((PdfNumeric)parameters["Predictor"]).ToInt32();
                }

                return columns <= 0
                    ? throw new NotImplementedException("The sample count must be greater than 0")
                    : FlateDecodeWithPredictor(predictor, columns, decompressed);
            }

            //else if (filter.Text == "DCTDecode")
            //{
            //    // JPEG image
            //}
            //else

            throw new NotImplementedException("Implement Filter: " + filter.Text);
        }

        internal static PdfStream Parse(PdfDictionary dictionary, Lexer lexer)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            lexer.Expects("stream");
            char eol = lexer.ReadChar();

            if (eol == '\r')
            {
                eol = lexer.ReadChar();
            }

            if (eol != '\n')
            {
                throw new ParsingException($@"Stream must end with either \r\n or \n. Was '{eol}'");
            }

            IPdfObject lengthObject = dictionary["Length"];

            if (lengthObject is null)
            {
                throw new ParsingException("Stream dictionary is missing 'Length' entry");
            }

            int length;
            if (lengthObject is PdfIndirectReference reference)
            {
                PdfIndirectObject lenobj = lexer.IndirectReferenceResolver
                    .GetObject(reference.ObjectNumber, reference.GenerationNumber);

                length = ((PdfNumeric)lenobj.Object).ToInt32();
            }
            else
            {
                length = int.Parse(lengthObject.ToString(), CultureInfo.InvariantCulture);
            }

            var data = PdfData.Parse(lexer, length);
            lexer.Expects("endstream");

            return new PdfStream(dictionary, data);
        }

        public override string ToString()
        {
            return "stream";
        }
    }
}
