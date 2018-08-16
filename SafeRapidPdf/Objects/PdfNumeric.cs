using System.Globalization;

namespace SafeRapidPdf.Objects
{
    public sealed class PdfNumeric : PdfObject
    {
        private readonly string text;

        private PdfNumeric(string text)
            : base(PdfObjectType.Numeric)
        {
            this.text = text;
        }

        public bool IsInteger => !IsReal;

        public bool IsReal => text.IndexOf('.') > -1;

        public static implicit operator double(PdfNumeric numeric)
        {
            return double.Parse(numeric.text, CultureInfo.InvariantCulture);
        }

        public static PdfNumeric Parse(Parsing.ILexer lexer)
        {
            return new PdfNumeric(lexer.ReadToken());
        }

        public static PdfNumeric Parse(string token)
        {
            return new PdfNumeric(token);
        }

        public long ToInt64()
        {
            return long.Parse(text, CultureInfo.InvariantCulture);
        }

        public int ToInt32()
        {
            return int.Parse(text, CultureInfo.InvariantCulture);
        }

        public decimal ToDecimal()
        {
            return decimal.Parse(text, CultureInfo.InvariantCulture);
        }

        public override string ToString() => text;
    }
}