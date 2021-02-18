using System;

namespace SafeRapidPdf.Parsing
{
    public class ParsingException : Exception
    {
        public ParsingException(string message)
            : base(message) { }

        public ParsingException()
        {
        }

        public ParsingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
