using System.Collections.Generic;

namespace SafeRapidPdf.File
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

        public static PdfIndirectObject Parse(Lexical.ILexer lexer)
        {
            int objectNumber = int.Parse(lexer.ReadToken());
            return Parse(lexer, objectNumber);
        }

        public static PdfIndirectObject Parse(Lexical.ILexer lexer, int objectNumber)
        {
            int generationNumber = int.Parse(lexer.ReadToken());
            lexer.Expects("obj");
            PdfObject obj = PdfObject.ParseAny(lexer);
            lexer.Expects("endobj");
            return new PdfIndirectObject(objectNumber, generationNumber, obj);
        }

        public int ObjectNumber { get; }

        public int GenerationNumber { get; }

        public IPdfObject Object { get; }

        public override string ToString()
        {
            return $"{ObjectNumber} {GenerationNumber} obj";
        }

        public override IReadOnlyList<IPdfObject> Items
        {
            get => new[] { Object };
        }
    }
}