using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	/// <summary>
	/// A  literal string is written as an arbitrary number of characters enclosed in 
	/// parentheses. Any characters may appear in a string except  unbalanced 
	/// parentheses and the backslash, which must be treated specially. Balanced pairs of 
	/// parentheses within a string require no special treatment. 
	/// </summary>
	public class PdfLiteralString : PdfString
	{
		public PdfLiteralString(IFileStructureParser parser)
		{
			int parenthesisCount = 0;
			StringBuilder sb = new StringBuilder();
			char c = parser.ReadChar();
			while (parenthesisCount != 0 || c != ')')
			{
				if (c == '(')
					parenthesisCount++;
				else if (c == ')')
					parenthesisCount--;
				sb.Append(c);
				c = parser.ReadChar();
			}
			Object = sb.ToString();
		}
	}
}
