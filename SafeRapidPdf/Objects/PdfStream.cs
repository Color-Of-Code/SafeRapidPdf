using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComponentAce.Compression.Libs.zlib;
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

        private byte[] FlateDecodeWithPredictorNone(int columns, byte[] decompressed)
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
                byte rowPredictor = (byte)decompressed[r * (columns + 1)];
                if (rowPredictor != 2)
                {
                    throw new NotImplementedException("Only up predictor is supported at the moment");
                }
                for (int i = 0; i < columns; i++)
                {
                    // the leading predictor is ignored, assuming it's always UP
                    var inputByte = (byte)decompressed[r * (columns + 1) + i + 1];
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
            switch (predictor)
            {
                case 1:  //1 = default: no prediction
                    return FlateDecodeWithPredictorNone(columns, input);
                case 12: //12 = PNG prediction (on encoding, PNG Up on all rows)
                    return FlateDecodeWithPredictorPngUp(columns, input);
                default:
                    throw new NotImplementedException($"Sorry at the moment predictor {predictor} is not implemented. Please make a feature request on https://github.com/jdehaan/SafeRapidPdf/issues. Ideally provide an example pdf.");
            }
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
                var zin = new ZInputStream(new MemoryStream(Data.Data));
                int r;
                var output = new List<byte>(32 * 1024); // avoid too many reallocs
                while ((r = zin.Read()) != -1)
                {
                    output.Add((byte)r);
                }
                byte[] decompressed = output.ToArray();
                zin.Close();

                // set defaults
                int predictor = 1; // no prediction
                int columns = 1;

                if (StreamDictionary.Keys.Contains("DecodeParms"))
                {
                    var parameters = (PdfDictionary)StreamDictionary["DecodeParms"];
                    columns = (PdfNumeric)parameters["Columns"];
                    predictor = (PdfNumeric)parameters["Predictor"];
                }

                if (columns <= 0)
                    throw new NotImplementedException("The sample count must be greater than 0");

                return FlateDecodeWithPredictor(predictor, columns, decompressed);
            }
            //else if (filter.Text == "DCTDecode")
            //{
            //    // JPEG image
            //}
            //else
            throw new NotImplementedException("Implement Filter: " + filter.Text);
        }

        public static PdfStream Parse(PdfDictionary dictionary, ILexer lexer)
        {
            if (dictionary == null)
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

            if (lengthObject == null)
            {
                throw new ParsingException("Stream dictionary is missing 'Length' entry");
            }

            int length = 0;
            if (lengthObject is PdfIndirectReference reference)
            {
                PdfIndirectObject lenobj = lexer.IndirectReferenceResolver
                    .GetObject(reference.ObjectNumber, reference.GenerationNumber);

                length = (PdfNumeric)lenobj.Object;
            }
            else
            {
                length = int.Parse(lengthObject.ToString());
            }

            var data = PdfData.Parse(lexer, length);
            lexer.Expects("endstream");

            return new PdfStream(dictionary, data);
        }

        public override string ToString() => "stream";
    }
}