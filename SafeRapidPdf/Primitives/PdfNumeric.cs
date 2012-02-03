using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfNumeric : PdfObject
	{
		public PdfNumeric(Lexical.ILexer lexer)
		{
			String token = lexer.ReadToken();
			Value = decimal.Parse(token, System.Globalization.CultureInfo.InvariantCulture);
		}

		public Decimal Value { get; private set; }

		public override string ToString()
		{
			return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}
