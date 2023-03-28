using System;
using System.IO;
using System.Text;
using SafeRapidPdf.Parsing;

namespace SafeRapidPdf.UnitTests.Util;


public static class StringExtensions
{
    public static Stream ToStream(this string input)
    {
        byte[] byteArray = Encoding.UTF8.GetBytes(input);
        return new MemoryStream(byteArray);
    }

    // used to inject a lexer into low level parsers from a string
    public static Lexer ToLexer(this string input)
    {
        return new Lexer(input.ToStream(), true);
    }

    // used to inject a lexer into low level parsers from a byte array
    public static Lexer ToLexer(this byte[] input)
    {
        var s = new MemoryStream(input);
        return new Lexer(s, true);
    }

    public static Lexer Base64ToLexer(this string input)
    {
        var bytes = Convert.FromBase64String(input);
        var s = new MemoryStream(bytes);
        return new Lexer(s, true);
    }

    public static string ToHexString(this byte[] ba)
    {
        if (ba == null) throw new ArgumentNullException(nameof(ba));

        var hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
        {
            _ = hex.AppendFormat("{0:x2}", b);
        }
        return hex.ToString();
    }
}
