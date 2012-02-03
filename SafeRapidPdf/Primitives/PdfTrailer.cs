using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfTrailer : PdfObject
	{
		public PdfTrailer(Lexical.ILexer lexer)
		{
			lexer.Expects("trailer");
			Content = new PdfDictionary(lexer);
		}

		public PdfDictionary Content { get; private set; }

	}
}
