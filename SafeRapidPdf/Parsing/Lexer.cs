using System.Collections.Generic;
using System.IO;
using System.Text;

using SafeRapidPdf.Services;

namespace SafeRapidPdf.Parsing
{
    public class Lexer
    {
        private static readonly bool[] _regularTable = new bool[257];
        private static readonly bool[] _whitespaceTable = new bool[257];
        private static readonly bool[] _delimiterTable = new bool[257];
        private readonly Stream _reader;
        private readonly Stack<long> _positions = new();
        private string _peekedToken;
        private string _peekedToken2;
        private int _byteRead = -1;

        static Lexer()
        {
            for (int c = 0; c < 257; c++)
            {
                _regularTable[c] = IsRegular(c - 1);
                _whitespaceTable[c] = IsWhitespace(c - 1);
                _delimiterTable[c] = IsDelimiter(c - 1);
            }
        }

        public Lexer(Stream stream, bool withoutResolver = false)
        {
            _reader = stream;
            _ = _reader.Seek(0, SeekOrigin.End);
            Size = _reader.Position;
            _ = _reader.Seek(0, SeekOrigin.Begin);

            if (!withoutResolver)
            {
                IndirectReferenceResolver = new IndirectReferenceResolver(this);
            }
        }

        public IIndirectReferenceResolver IndirectReferenceResolver { get; private set; }

        public int Percentage => (int)(_reader.Position * 100 / Size);

        public long Size { get; }

        public void Expects(string expectedToken)
        {
            string actualToken = ReadToken();

            if (actualToken != expectedToken)
            {
                throw new UnexpectedTokenException(expectedToken, actualToken);
            }
        }

        public string PeekToken2()
        {
            _peekedToken ??= ReadTokenInternal();

            if (IsInteger(_peekedToken))
            {
                _peekedToken2 ??= ReadTokenInternal();

                // should be "obj" or "R"
                string token = _peekedToken2;
                if (token is "obj" or "R")
                {
                    return token;
                }
            }
            return _peekedToken;
        }

        public string PeekToken1()
        {
            _peekedToken ??= ReadTokenInternal();

            return _peekedToken;
        }

        public string ReadToken()
        {
            if (_peekedToken != null)
            {
                string peekedToken = _peekedToken;
                _peekedToken = _peekedToken2;
                _peekedToken2 = null;
                return peekedToken;
            }
            return ReadTokenInternal();
        }

        public string ReadUntilEol()
        {
            var sb = new StringBuilder();

            while (true)
            {
                int c = ReadByte();

                if (IsEol(c))
                {
                    break;
                }

                _ = sb.Append((char)c);
            }

            return sb.ToString();
        }

        public byte[] ReadBytes(int length)
        {
            byte[] buffer = new byte[length];

            return _reader.Read(buffer, 0, length) != length
                ? throw new ParsingException("Could not read the full amount of bytes")
                : buffer;
        }

        public char ReadChar()
        {
            return (char)ReadByte();
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

        private string ReadTokenInternal()
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

            return string.IsNullOrEmpty(token)
                ? ReadByte() == -1
                    ? null
                    : throw new ParsingException("Token may not be empty")
                : token;
        }

        private string ParseToken(int b)
        {
            var token = new StringBuilder();
            if (_delimiterTable[b + 1])
            {
                _ = token.Append((char)b);
                b = ReadByte();
            }
            else
            {
                while (_regularTable[b + 1])
                {
                    _ = token.Append((char)b);
                    b = ReadByte();
                }
            }
            _byteRead = b;

            return token.ToString();
        }

        /// <summary>
        /// Skip whitespaces and return the first non-whitespace char
        /// </summary>
        /// <returns></returns>
        private int SkipWhitespaces()
        {
            int c;
            do
            {
                c = ReadByte();
            } while (_whitespaceTable[c + 1]);

            return c;
        }

        public void Putc()
        {
            _ = _reader.Seek(-1, SeekOrigin.Current);
        }

        private static bool IsInteger(string token)
        {
            return int.TryParse(token, out _);
        }

        /// <summary>
        /// Whitespace as defined by PDF
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsWhitespace(int b)
        {
            return b is <= 32 and // shortcut everything > 32 => most cases
                (32 or 10 or 12 or 13 or 9 or 0);
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
                b is '/' or       // 47
                '<' or '>' or     // 60 62
                '[' or ']' or     // 91 93
                '(' or ')' or     // 40 41
                '{' or '}' or     // 123 125
                '%';              // 37
        }

        /// <summary>
        /// End of line as defined by PDF
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsEol(int b)
        {
            // -1 was added to catch %%EOF without CR or LF
            return b is 10 or 13 or (-1);
        }

        public void PushPosition(long newPosition)
        {
            _positions.Push(_reader.Position);
            _ = newPosition < 0
                ? _reader.Seek(newPosition, SeekOrigin.End)
                : _reader.Seek(newPosition, SeekOrigin.Begin);

            _peekedToken = null;
            _peekedToken2 = null;
        }

        public void PopPosition()
        {
            _ = _reader.Seek(_positions.Pop(), SeekOrigin.Begin);
            _peekedToken = null;
            _peekedToken2 = null;
        }
    }
}
