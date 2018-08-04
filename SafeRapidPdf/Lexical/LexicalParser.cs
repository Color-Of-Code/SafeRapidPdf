using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SafeRapidPdf.Lexical
{
    /// <summary>
    /// The lexer
    /// </summary>
    internal class LexicalParser : ILexer
	{
		private static bool[] _regularTable = new bool[257];
		private static bool[] _whitespaceTable = new bool[257];
		private static bool[] _delimiterTable = new bool[257];
		static LexicalParser()
		{
			for (int c = 0; c < 257; c++)
			{
				_regularTable[c] = IsRegular(c-1);
				_whitespaceTable[c] = IsWhitespace(c-1);
				_delimiterTable[c] = IsDelimiter(c-1);
			}
		}

		public LexicalParser(Stream stream)
		{
			_reader = stream;
			_reader.Seek(0, SeekOrigin.End);
			_size = _reader.Position;
			_reader.Seek(0, SeekOrigin.Begin);
			IndirectReferenceResolver = new ObjectResolver.IndirectReferenceResolver(this);
		}

		public void Expects(String expectedToken)
		{
			string actualToken = ReadToken();
            if (actualToken != expectedToken)
                throw new Exception($"Parser error: expected '{expectedToken}', read '{actualToken}'");
		}

		private String _peekedToken;
		private String _peekedToken2;

		public String PeekToken2()
		{
			_peekedToken = _peekedToken ?? ReadTokenInternal();
			if (IsInteger(_peekedToken))
			{
				_peekedToken2 = _peekedToken2 ?? ReadTokenInternal();
				// should be "obj" or "R"
				string token = _peekedToken2;
				if (token == "obj" || token == "R")
				{
					return token;
				}
			}
			return _peekedToken;
		}

		public String PeekToken1()
		{
			_peekedToken = _peekedToken ?? ReadTokenInternal();
			return _peekedToken;
		}

		public String ReadToken()
		{
			if (_peekedToken != null)
			{
				String peekedToken = _peekedToken;
				_peekedToken = _peekedToken2;
				_peekedToken2 = null;
				return peekedToken;
			}
			return ReadTokenInternal();
		}

		private String ReadTokenInternal()
		{
			int b = SkipWhitespaces();
			if (b == -1)
				return null;

			int c = b;
			switch (c)
			{
				case '%': return "%";
				case '/': return "/";
				case '[': return "[";
				case ']': return "]";
				case '(': return "(";
				case ')': return ")";
				case '<':
					b = ReadByte();
					if (b == '<')
						return "<<";
					_byteRead = b;
					return "<";
				case '>':
					b = ReadByte();
					if (b == '>')
						return ">>";
					_byteRead = b;
					return ">";
			}

			string token = ParseToken(c);

			if (token == String.Empty)
			{
				if (ReadByte() == -1) // end of file
					return null;
				throw new Exception("Token may not be empty");
			}
			return token;
		}

		private int _byteRead = -1;

		public String ReadUntilEol()
		{
			var sb = new StringBuilder();
			while (true)
			{
				int c = ReadByte();
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

		private int ReadByte()
		{
			if (_byteRead != -1)
			{
				int result = _byteRead;
				_byteRead = -1;
				return result;
			}
			return _reader.ReadByte();
		}

		public char ReadChar()
		{
			return (char)ReadByte();
		}

		private String ParseToken(int b)
		{
			var token = new StringBuilder();
			if (_delimiterTable[b+1])
			{
				token.Append((char)b);
				b = ReadByte();
			}
			else
			{
				while (_regularTable[b+1])
				{
					token.Append((char)b);
					b = ReadByte();
				}
			}
			_byteRead = b;
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
				c = ReadByte();
			} while (_whitespaceTable[c+1]);
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
			return (b <= 32) && // shortcut everything > 32 => most cases
				(b == 32 || b == 10 || b == 12 || b == 13|| b == 9  || b == 0);
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
			// 37 40 41 47 60 62 91 93 123 125
			return
				b == '/' ||					// 47
				b == '<' || b == '>' ||		// 60 62
				b == '[' || b == ']' ||		// 91 93
				b == '(' || b == ')' ||		// 40 41
				b == '{' || b == '}' ||		// 123 125
				b == '%';					// 37
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

		public IIndirectReferenceResolver IndirectReferenceResolver { get; private set; }

		private Stack<long> _positions = new Stack<long>();
		private long _size;

		public long Size
		{
			get { return _size; }
		}

		public void PushPosition(long newPosition)
		{
			_positions.Push(_reader.Position);
			if (newPosition < 0)
				_reader.Seek(newPosition, SeekOrigin.End);
			else
				_reader.Seek(newPosition, SeekOrigin.Begin);
			_peekedToken = null;
			_peekedToken2 = null;
		}

		public void PopPosition()
		{
			_reader.Seek(_positions.Pop(), SeekOrigin.Begin);
			_peekedToken = null;
			_peekedToken2 = null;
		}

		public int Percentage
		{
			get
			{
				return (int)(_reader.Position * 100 / _size);
			}
		}

	}
}