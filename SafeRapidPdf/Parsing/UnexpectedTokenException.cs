namespace SafeRapidPdf.Parsing;

public class UnexpectedTokenException : ParsingException
{
    public UnexpectedTokenException(string expectedToken, string actualToken)
        : base($"Expected '{expectedToken}'. Was '{actualToken}'") { }

    public UnexpectedTokenException()
    {
    }

    public UnexpectedTokenException(string message)
        : base(message)
    {
    }

    public UnexpectedTokenException(string message, System.Exception innerException)
        : base(message, innerException)
    {
    }
}
