
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

        // used to inject a lexer into low level parsers from a byte array
        public static ILexer ToLexer(this Byte[] input)
        {
            var s = new SIO.MemoryStream(input);
            return new LexicalParser(s);
        }
        public static ILexer Base64ToLexer(this String input)
        {
            var bytes = Convert.FromBase64String(input);
            var s = new SIO.MemoryStream(bytes);
            return new LexicalParser(s);
        }

        public static string ToHexString(this byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}