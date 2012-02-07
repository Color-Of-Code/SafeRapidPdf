using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.File
{
	/// <summary>
	/// A  literal string is written as an arbitrary number of characters enclosed in 
	/// parentheses. Any characters may appear in a string except  unbalanced 
	/// parentheses and the backslash, which must be treated specially. Balanced pairs of 
	/// parentheses within a string require no special treatment. 
	/// </summary>
	public class PdfLiteralString : PdfObject
	{
		private PdfLiteralString(String text)
			: base(PdfObjectType.LiteralString)
		{
			_text = text;
		}

		public static PdfLiteralString Parse(Lexical.ILexer lexer)
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
				if (c == '\\')
				{
					c = lexer.ReadChar();
					switch (c)
					{
						case 'n':
							sb.Append("\n");
							break;
						case 'r':
							sb.Append("\r");
							break;
						case 't':
							sb.Append("\t");
							break;
						case 'f':
							sb.Append("\f");
							break;
						// \b Backspace (BS)
						case 'b':
							throw new NotImplementedException("Backspace char parsing");

						case '\\':
						case ')':
						case '(':
							sb.Append(c);
							break;

						case '\r':
							break;

						default:
							// \ddd Character code ddd (octal)
							StringBuilder octalNumber = new StringBuilder();
							octalNumber.Append(c);
							char c2 = lexer.ReadChar();
							if (!Char.IsDigit(c2))
								lexer.Putc();
							else
							{
								octalNumber.Append(c2);
								char c3 = lexer.ReadChar();
								if (!Char.IsDigit(c3))
									lexer.Putc();
								else
									octalNumber.Append(c2);
							}
							int octal = Convert.ToInt32(octalNumber.ToString(), 8);
							sb.Append((char)octal);
							break;
					}
				}
				else
				{
					sb.Append(c);
				}
				c = lexer.ReadChar();
			}
			return new PdfLiteralString(sb.ToString());
		}

		private String _text;

		public override string ToString()
		{
			return _text;
		}
	}
}
