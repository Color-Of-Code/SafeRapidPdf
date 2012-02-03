using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfStartXRef : PdfObject
	{
		public PdfStartXRef(Lexical.ILexer lexer)
		{
			lexer.Expects("startxref");
			Numeric = new PdfNumeric(lexer);
		}

		public PdfNumeric Numeric { get; private set; }

		public override string ToString()
		{
			return String.Format("startxref {0}", Numeric);
		}
	}
}
