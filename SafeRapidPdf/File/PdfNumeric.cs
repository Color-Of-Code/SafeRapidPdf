using System;
using System.Globalization;

namespace SafeRapidPdf.File
{
    public sealed class PdfNumeric : PdfObject
    {
        private PdfNumeric(decimal value)
            : base(PdfObjectType.Numeric)
        {
            Value = value;
        }

        public decimal Value { get; }

        public bool IsInteger => (Value % 1) == 0;

        public static PdfNumeric Parse(Lexical.ILexer lexer)
        {
            return Parse(lexer.ReadToken());
        }

        public static PdfNumeric Parse(string token)
        {
            var value = decimal.Parse(token, CultureInfo.InvariantCulture);
            return new PdfNumeric(value);
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
