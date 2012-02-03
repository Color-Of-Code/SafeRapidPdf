using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	/// <summary>
	/// Comments start with % and end on EOL char (CR or LF)
	/// </summary>
	public class PdfComment : PdfObject
	{
		internal PdfComment(Lexical.ILexer lexer)
		{
			lexer.Expects("%");
			Text = lexer.ReadUntilEol();
		}

		public String Text { get; private set; }

		public bool IsEOF
		{
			get
			{
				return Text == "%EOF";
			}
		}

		public override string ToString()
		{
			return String.Format("%{0}", Text);
		}
	}
}
