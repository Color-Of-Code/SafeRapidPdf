using System.Collections.Generic;

namespace SafeRapidPdf.Objects
{
    public sealed class PdfStartXRef : PdfObject
    {
        private PdfStartXRef(PdfNumeric value)
            : base(PdfObjectType.StartXRef)
        {
            IsContainer = true;
            Numeric = value;
        }

        public PdfNumeric Numeric { get; }

        public override IReadOnlyList<IPdfObject> Items => new[] { Numeric };

        public static PdfStartXRef Parse(Lexical.ILexer lexer)
        {
            var n = PdfNumeric.Parse(lexer);
            return new PdfStartXRef(n);
        }

        public override string ToString() => "startxref";
    }
}