
namespace SafeRapidPdf.UnitTests.Util
{
    using System;
    using System.IO;
    using System.Text;
    using SafeRapidPdf.Parsing;

    public static class StringExtensions
    {
        public static Stream ToStream(this string input)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            return new MemoryStream(byteArray);
        }

        // used to inject a lexer into low level parsers from a string
        public static ILexer ToLexer(this string input)
        {
            return new LexicalParser(input.ToStream(), true);
        }

        // used to inject a lexer into low level parsers from a byte array
        public static ILexer ToLexer(this byte[] input)
        {
            var s = new MemoryStream(input);
            return new LexicalParser(s, true);
        }

        public static ILexer Base64ToLexer(this string input)
        {
            var bytes = Convert.FromBase64String(input);
            var s = new MemoryStream(bytes);
            return new LexicalParser(s, true);
        }

        public static string ToHexString(this byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
    }
}