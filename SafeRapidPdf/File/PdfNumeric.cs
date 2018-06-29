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

		public static PdfNumeric Parse(Lexical.ILexer lexer)
		{
			return Parse(lexer.ReadToken());
		}

		public static PdfNumeric Parse(String token)
		{
			var value = decimal.Parse(token, CultureInfo.InvariantCulture);
			return new PdfNumeric(value);
		}

		public Decimal Value { get; private set; }

		public Boolean IsInteger => (Value % 1) == 0;

        public override string ToString()
		{
			return Value.ToString(CultureInfo.InvariantCulture);
		}
	}
}
