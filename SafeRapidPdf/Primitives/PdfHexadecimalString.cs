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
			String text = lexer.ReadToken();
			String cleantext = String.Join("", text.Where(x => !Lexical.LexicalParser.IsWhitespace(x)));
			if ((cleantext.Length % 2) != 0)
				cleantext = cleantext + "0";
			//int length = cleantext.Length;
			//StringBuilder sb = new StringBuilder();
			//for (int i = 0; i < length; i += 2)
			//{
			//    byte b = Convert.ToByte(cleantext.Substring(i, 2), 16);
			//    sb.Append((char)b);
			//}
			//_text = sb.ToString();
			_text = cleantext;

			lexer.Expects(">");
		}
	}
}
