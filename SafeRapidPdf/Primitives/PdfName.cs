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
			Name = lexer.ReadToken();
		}

		public string Name { get; private set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
