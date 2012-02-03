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
			_text = lexer.ReadToken();
			lexer.Expects(">");
		}

		private String _text;

		public override string ToString()
		{
			return String.Format("<{0}>", _text);
		}
	}
}
