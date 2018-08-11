using System.Collections.Generic;

namespace SafeRapidPdf.File
{
    public sealed class PdfStartXRef : PdfObject
    {
        private PdfStartXRef(PdfNumeric value)
            : base(PdfObjectType.StartXRef)
        {
            IsContainer = true;
            Numeric = value;
        }

        public static PdfStartXRef Parse(Lexical.ILexer lexer)
        {
            var n = PdfNumeric.Parse(lexer);
            return new PdfStartXRef(n);
        }

        public PdfNumeric Numeric { get; }

        public override IReadOnlyList<IPdfObject> Items
        {
            get => new[] { Numeric };
        }

        public override string ToString()
        {
            return "startxref";
        }
    }
}