namespace SafeRapidPdf.Parsing
{
    public class UnexpectedTokenException : ParsingException
    {
        public UnexpectedTokenException(string expectedToken, string actualToken)
            : base($"Expected '{expectedToken}'. Was '{actualToken}'") { }

    }
}