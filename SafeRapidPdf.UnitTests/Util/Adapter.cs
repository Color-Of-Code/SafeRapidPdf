
using System;
using System.Text;
using SafeRapidPdf.Lexical;
using SIO = System.IO;

namespace SafeRapidPdf.UnitTests.Util
{
    public static class StringExtensions
    {
        public static SIO.Stream ToStream(this String input)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            return new SIO.MemoryStream(byteArray);
        }

        // used to inject a lexer into low level parsers from a string
        public static ILexer ToLexer(this String input)
        {
            return new LexicalParser(input.ToStream());
        }
    }
}