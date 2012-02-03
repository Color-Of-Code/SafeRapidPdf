using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfName : PdfObject
	{
		public PdfName(Lexical.ILexer lexer)
		{
			lexer.Expects("/");
			Text = lexer.ReadToken();
		}

		public string Text { get; private set; }

		public override string ToString()
		{
			return String.Format("/{0}", Text);
		}
	}
}
