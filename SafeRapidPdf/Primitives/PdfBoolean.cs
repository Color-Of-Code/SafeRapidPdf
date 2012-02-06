using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfBoolean : PdfObject
	{
		private PdfBoolean(Boolean value)
			: base(PdfObjectType.Boolean)
		{
			Value = value;
		}

		public static PdfBoolean Parse(Lexical.ILexer lexer)
		{
			String token = lexer.ReadToken();
			if (token != "true" && token != "false")
				throw new Exception("Parser error: invalid boolean value");
			return new PdfBoolean(token == "true");
		}

		public Boolean Value { get; private set; }

		public override string ToString()
		{
			return String.Format("{0}", Value ? "true" : "false");
		}
	}
}
