using System;
using System.Collections.Generic;
using SafeRapidPdf.Parsing;

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

        public static PdfStartXRef Parse(Lexer lexer)
        {
            if (lexer is null)
            {
                throw new ArgumentNullException(nameof(lexer));
            }

            var n = PdfNumeric.Parse(lexer);
            return new PdfStartXRef(n);
        }

        public override string ToString()
        {
            return "startxref";
        }
    }
}
