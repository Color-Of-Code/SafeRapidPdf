using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfHexadecimalString : PdfString
	{
		public PdfHexadecimalString(Lexical.ILexer lexer)
		{
			lexer.Expects("<");
			Text = lexer.ReadToken();
			lexer.Expects(">");
		}

		public String Text { get; private set; }

		public override string ToString()
		{
			return String.Format("<{0}>", Text);
		}
	}
}
