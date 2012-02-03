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
		public PdfLiteralString(Lexical.ILexer lexer)
		{
			lexer.Expects("(");
			int parenthesisCount = 0;
			StringBuilder sb = new StringBuilder();
			char c = lexer.ReadChar();
			while (parenthesisCount != 0 || c != ')')
			{
				if (c == '(')
					parenthesisCount++;
				else if (c == ')')
					parenthesisCount--;
				sb.Append(c);
				if (c == '\\')
				{
					c = lexer.ReadChar();
					sb.Append(c);
				}
				c = lexer.ReadChar();
			}
			_text = sb.ToString();
		}

		private String _text;

		public override string ToString()
		{
			return _text;
		}
	}
}
