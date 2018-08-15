using System;

namespace SafeRapidPdf.Parsing
{
    public class ParsingException : Exception
    {
        public ParsingException(string message)
            : base(message) { }
    }
}