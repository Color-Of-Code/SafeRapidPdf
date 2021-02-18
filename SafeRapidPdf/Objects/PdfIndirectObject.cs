using System.Collections.Generic;
using System.Globalization;
using SafeRapidPdf.Parsing;

namespace SafeRapidPdf.Objects
{
    public sealed class PdfIndirectObject : PdfObject
    {
        private PdfIndirectObject(int objectNumber, int generationNumber, IPdfObject obj)
            : base(PdfObjectType.IndirectObject)
        {
            IsContainer = true;

            ObjectNumber = objectNumber;
            GenerationNumber = generationNumber;
            Object = obj;
        }

        public int ObjectNumber { get; }

        public int GenerationNumber { get; }

        public IPdfObject Object { get; }

        public override IReadOnlyList<IPdfObject> Items => new[] { Object };

        public static PdfIndirectObject Parse(Lexer lexer)
        {
            int objectNumber = int.Parse(lexer.ReadToken(), CultureInfo.InvariantCulture);
            return Parse(lexer, objectNumber);
        }

        public static PdfIndirectObject Parse(Lexer lexer, int objectNumber)
        {
            int generationNumber = int.Parse(lexer.ReadToken(), CultureInfo.InvariantCulture);
            lexer.Expects("obj");
            PdfObject obj = ParseAny(lexer);
            lexer.Expects("endobj");
            return new PdfIndirectObject(objectNumber, generationNumber, obj);
        }

        public override string ToString()
        {
            return $"{ObjectNumber} {GenerationNumber} obj";
        }
    }
}
