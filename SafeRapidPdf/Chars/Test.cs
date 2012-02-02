using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Chars
{
	/// <summary>
	/// The character classification defined by PDF
	/// </summary>
	public static class Test
	{

		/// <summary>
		/// Whitespace as defined by PDF
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool IsWhitespace(int b)
		{
			return b == 0 || b == 9 || b == 10 || b == 12 || b == 13 || b == 32;
		}

		/// <summary>
		/// Regular char as defined by PDF
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool IsRegular(int b)
		{
			return !IsWhitespace(b) && !IsDelimiter(b) && b != -1;
		}

		/// <summary>
		/// Delimiter char as defined by PDF
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool IsDelimiter(int b)
		{
			return
				b == '(' || b == ')' ||
				b == '<' || b == '>' ||
				b == '[' || b == ']' ||
				b == '{' || b == '}' ||
				b == '/' || b == '%';
		}

		/// <summary>
		/// End of line as defined by PDF
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool IsEol(int b)
		{
			// -1 was added to catch %%EOF without CR or LF
			return b == 10 || b == 13 || b == -1;
		}

	}
}
