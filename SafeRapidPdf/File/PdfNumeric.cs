using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.File
{
	public class PdfNumeric : PdfObject
	{
		private PdfNumeric(decimal value)
			: base(PdfObjectType.Numeric)
		{
			Value = value;
		}

		public static PdfNumeric Parse(Lexical.ILexer lexer)
		{
			String token = lexer.ReadToken();
			var value = decimal.Parse(token, System.Globalization.CultureInfo.InvariantCulture);
			return new PdfNumeric(value);
		}

		public Decimal Value { get; private set; }

		public override string ToString()
		{
			return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}
