using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfNull : PdfObject
	{
		private PdfNull()
			: base(PdfObjectType.Null)
		{
		}

		private static readonly PdfNull Null = new PdfNull();

		public static PdfNull Parse(Lexical.ILexer lexer)
		{
			lexer.Expects("null");
			return Null;
		}

		public override string ToString()
		{
			return "null";
		}
	}
}
