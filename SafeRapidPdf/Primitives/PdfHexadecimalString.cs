using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfHexadecimalString : PdfObject
	{
		private PdfHexadecimalString(String hexString)
		{
			_text = hexString;
			//int length = cleantext.Length;
			//StringBuilder sb = new StringBuilder();
			//for (int i = 0; i < length; i += 2)
			//{
			//    byte b = Convert.ToByte(cleantext.Substring(i, 2), 16);
			//    sb.Append((char)b);
			//}
			//_text = sb.ToString();
		}

		public static PdfHexadecimalString Parse(Lexical.ILexer lexer)
		{
			lexer.Expects("<");
			StringBuilder hexString = new StringBuilder();
			String text = string.Empty;
			while (text != ">")
			{
				hexString.Append(text);
				text = lexer.ReadToken();
			}
			if ((hexString.Length % 2) != 0)
				hexString.Append('0');
			return new PdfHexadecimalString(hexString.ToString());
		}

		private String _text;

		public override string ToString()
		{
			return _text;
		}
	}
}
