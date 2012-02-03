using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfNull : PdfObject
	{
		public PdfNull(Lexical.ILexer lexer)
		{
			lexer.Expects("null");
		}

		public override string ToString()
		{
			return "null";
		}
	}
}
