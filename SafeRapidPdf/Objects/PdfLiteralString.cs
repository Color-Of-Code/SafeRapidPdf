using System;
using System.Text;

namespace SafeRapidPdf.Objects
{
    /// <summary>
    /// A  literal string is written as an arbitrary number of characters enclosed in
    /// parentheses. Any characters may appear in a string except  unbalanced
    /// parentheses and the backslash, which must be treated specially. Balanced pairs of
    /// parentheses within a string require no special treatment.
    /// </summary>
    public sealed class PdfLiteralString : PdfObject
    {
        private readonly string _text;

        private PdfLiteralString(string text)
            : base(PdfObjectType.LiteralString)
        {
            _text = text;
        }

        public static PdfLiteralString Parse(Parsing.Lexer lexer)
        {
            int parenthesisCount = 0;
            var sb = new StringBuilder();
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
                            _ = sb.Append('\n');
                            break;
                        case 'r':
                            _ = sb.Append('\r');
                            break;
                        case 't':
                            _ = sb.Append('\t');
                            break;
                        case 'f':
                            _ = sb.Append('\f');
                            break;

                        // \b Backspace (BS)
                        case 'b':
                            throw new NotImplementedException("Backspace char parsing");

                        case '\\':
                        case ')':
                        case '(':
                            _ = sb.Append(c);
                            break;

                        case '\r':
                            break;

                        default:
                            // \ddd Character code ddd (octal)
                            var octalNumber = new StringBuilder();
                            _ = octalNumber.Append(c);
                            char c2 = lexer.ReadChar();
                            if (!char.IsDigit(c2))
                            {
                                lexer.Putc();
                            }
                            else
                            {
                                _ = octalNumber.Append(c2);
                                char c3 = lexer.ReadChar();
                                if (!char.IsDigit(c3))
                                    lexer.Putc();
                                else
                                    _ = octalNumber.Append(c2);
                            }
                            int octal = Convert.ToInt32(octalNumber.ToString(), 8);
                            _ = sb.Append((char)octal);
                            break;
                    }
                }
                else
                {
                    _ = sb.Append(c);
                }
                c = lexer.ReadChar();
            }
            return new PdfLiteralString(sb.ToString());
        }

        public override string ToString()
        {
            return _text;
        }
    }
}
