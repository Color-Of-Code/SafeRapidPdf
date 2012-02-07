using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Lexical
{
	/// <summary>
	/// The lexer
	/// </summary>
	internal class LexicalParser : ILexer
	{
		public LexicalParser(Stream stream)
		{
			_reader = stream;
			IndirectReferenceResolver = new ObjectResolver.IndirectReferenceResolver(this);
		}

		public void Expects(String expectedToken)
		{
			String actualToken = ReadToken();
			if (actualToken != expectedToken)
				throw new Exception(String.Format("Parser error: expected '{0}', read '{1}'",
					expectedToken, actualToken));
		}

		public String PeekToken()
		{
			long lastPosition = _reader.Position;
			String token = ReadToken();
			if (IsInteger(token))
			{
				string token2 = ParseToken();
				if (IsInteger(token2))
				{
					// should be "obj" or "R"
					string token3 = ParseToken();
					if (token3 == "obj" || token3 == "R")
					{
						token = token3;
					}
				}
			}
			_reader.Seek(lastPosition, SeekOrigin.Begin);
			return token;
		}

		public String ReadToken()
		{
			int b = SkipWhitespaces();
			if (b == -1)
				return null;

			char c = (char)b;
			switch (c)
			{
				case '%': return "%";
				case '/': return "/";
				case '[': return "[";
				case ']': return "]";
				case '(': return "(";
				case ')': return ")";
				case '<':
					b = _reader.ReadByte();
					if ((char)b == '<')
						return "<<";
					Putc();
					return "<";
				case '>':
					b = _reader.ReadByte();
					if ((char)b == '>')
						return ">>";
					Putc();
					return ">";
			}

			string token = ParseToken(c);

			if (token == String.Empty)
			{
				if (_reader.ReadByte() == -1) // end of file
					return null;
				throw new Exception("Token may not be empty");
			}

			// one step back because one char has been read too far
			Putc();

			return token;
		}

		public String ReadUntilEol()
		{
			StringBuilder sb = new StringBuilder();
			while (true)
			{
				int c = _reader.ReadByte();
				if (IsEol(c))
					break;
				sb.Append((char)c);
			}
			return sb.ToString();
		}

		public byte[] ReadBytes(int length)
		{
			byte[] buffer = new byte[length];
			if (_reader.Read(buffer, 0, length) != length)
				throw new Exception("Parser error: could not read the full amount of bytes");
			return buffer;
		}

		public char ReadChar()
		{
			return (char)_reader.ReadByte();
		}

		private String ParseToken(int b)
		{
			StringBuilder token = new StringBuilder();
			if (IsDelimiter(b))
			{
				token.Append((char)b);
				b = _reader.ReadByte();
			}
			else
			{
				while (IsRegular(b))
				{
					token.Append((char)b);
					b = _reader.ReadByte();
				}
			}
			return token.ToString();
		}

		private String ParseToken()
		{
			int b = SkipWhitespaces();
			return ParseToken(b);
		}

		/// <summary>
		/// Skip whitespaces and return the first non
		/// whitespace char
		/// </summary>
		/// <returns></returns>
		private int SkipWhitespaces()
		{
			int c = 0;
			do
			{
				c = _reader.ReadByte();
			} while (IsWhitespace(c));
			return c;
		}

		public void Putc()
		{
			_reader.Seek(-1, SeekOrigin.Current);
		}

		private static bool IsInteger(string token)
		{
			int result;
			if (int.TryParse(token, out result))
				return true;
			return false;
		}

		private Stream _reader;

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

		public void SkipEol()
		{
			while (IsEol(_reader.ReadByte())) ;
			Putc();
		}

		public IIndirectReferenceResolver IndirectReferenceResolver { get; private set; }

		private Stack<long> _positions = new Stack<long>();

		public void PushPosition(long newPosition)
		{
			_positions.Push(_reader.Position);
			if (newPosition < 0)
				_reader.Seek(newPosition, SeekOrigin.End);
			else
				_reader.Seek(newPosition, SeekOrigin.Begin);
		}

		public void PopPosition()
		{
			_reader.Seek(_positions.Pop(), SeekOrigin.Begin);
		}
	}
}
