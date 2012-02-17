using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.File
{
	/// <summary>
	/// Comments start with % and end on EOL char (CR or LF)
	/// </summary>
	public class PdfComment : PdfObject
	{
		private PdfComment(String text)
			: base(PdfObjectType.Comment)
		{
			_text = text;
		}

		public static PdfComment Parse(Lexical.ILexer lexer)
		{
			return new PdfComment(lexer.ReadUntilEol());
		}

		private String _text;

		public bool IsEOF
		{
			get
			{
				return _text == "%EOF";
			}
		}

		public override string ToString()
		{
			return String.Format("%{0}", _text);
		}
	}
}
