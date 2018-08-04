
using System;
using System.Text;
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
    }
}