using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ComponentAce.Compression.Libs.zlib;

namespace SafeRapidPdf.File
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

        public byte[] Decode()
        {
            //TODO: multiple filter in order can be specified
            IPdfObject filter = StreamDictionary["Filter"];
            if (filter.Text == "FlateDecode")
            {
                PdfDictionary parameters = StreamDictionary["DecodeParms"] as PdfDictionary;
                var samplesPerRow = parameters["Columns"] as PdfNumeric;
                //12 = PNG prediction (on encoding, PNG Up on all rows)
                var predictor = parameters["Predictor"] as PdfNumeric;

                var zin = new ZInputStream(new MemoryStream(Data.Data));
                int r;
                var output = new List<Byte>(32 * 1024); // avoid too many reallocs
                while ((r = zin.Read()) != -1)
                {
                    output.Add((Byte)r);
                }
                byte[] decompressed = output.ToArray();
                zin.Close();

                // now we have to handle the predictors...
                if (predictor.Value != 12)
                    throw new NotImplementedException("Sorry at the moment only PNG prediction all UP is implemented");
                int samples = (int)samplesPerRow.Value;
                if (samples <= 0)
                    throw new NotImplementedException("The sample count must be greater than 0");

                output.Clear();
                var previousRow = new Byte[samples];
                for (int i = 0; i < samples; i++)
                    previousRow[i] = 0;
                int rows = decompressed.Length / (samples + 1); // we have an additional predictor byte in the source
                for (r = 0; r < rows; r++)
                {
                    var currentRow = new Byte[samples];
                    byte rowPredictor = (Byte)decompressed[r * (samples + 1)];
                    if (rowPredictor != 2)
                        throw new Exception("Only up predictor is supported at the moment");
                    for (int i = 0; i < samples; i++)
                    {
                        // the leading predictor is ignored, assuming it's always UP
                        var inputByte = (Byte)decompressed[r * (samples + 1) + i + 1];
                        currentRow[i] = (Byte)(inputByte + previousRow[i]);
                        output.Add(currentRow[i]);
                    }
                    previousRow = currentRow;
                }

                return output.ToArray();
            }
            //else if (filter.Text == "DCTDecode")
            //{
            //    // JPEG image
            //}
            //else
            throw new NotImplementedException("Implement Filter: " + filter.Text);
        }

        public static PdfStream Parse(PdfDictionary dictionary, Lexical.ILexer lexer)
        {
            lexer.Expects("stream");
            char eol = lexer.ReadChar();
            if (eol == '\r')
                eol = lexer.ReadChar();
            if (eol != '\n')
                throw new Exception(@"Parser error: stream needs to be followed by either \r\n or \n alone");

            if (dictionary == null)
                throw new Exception("Parser error: stream needs a dictionary");

            IPdfObject lengthObject = dictionary["Length"];
            if (lengthObject == null)
                throw new Exception("Parser error: stream dictionary requires a Length entry");

            int length = 0;
            if (lengthObject is PdfIndirectReference reference)
            {
                PdfIndirectObject lenobj = lexer.IndirectReferenceResolver
                    .GetObject(reference.ObjectNumber, reference.GenerationNumber);

                PdfNumeric len = (PdfNumeric)lenobj.Object;
                length = int.Parse(len.ToString());
            }
            else
            {
                length = int.Parse(lengthObject.ToString());
            }

            PdfData data = PdfData.Parse(lexer, length);
            lexer.Expects("endstream");

            return new PdfStream(dictionary, data);
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
                return list.AsReadOnly();
            }
        }

        public override string ToString() => "stream";
    }
}