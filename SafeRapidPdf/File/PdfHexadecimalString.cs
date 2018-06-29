using System;
using System.Text;

namespace SafeRapidPdf.File
{
    public sealed class PdfHexadecimalString : PdfObject
	{
		private PdfHexadecimalString(string hexString)
			: base(PdfObjectType.HexadecimalString)
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
			var hexString = new StringBuilder();
			string text = string.Empty;
			while (text != ">")
			{
				hexString.Append(text);
				text = lexer.ReadToken();
			}
			if ((hexString.Length % 2) != 0)
				hexString.Append('0');
			return new PdfHexadecimalString(hexString.ToString());
		}

		private string _text;

        public override string ToString() => _text;
	}
}
