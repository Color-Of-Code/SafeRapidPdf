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
		internal PdfComment(Stream s)
		{
			StringBuilder sb = new StringBuilder();
			while (true)
			{
				int c = s.ReadByte();
				if (Chars.Test.IsEol(c))
					break;
				sb.Append((char)c);
			}
			Object = sb.ToString();
		}

		public String Text
		{
			get
			{
				return Object as String;
			}
		}

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
