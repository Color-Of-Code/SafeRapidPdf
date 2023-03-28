namespace SafeRapidPdf.Objects;

public sealed class PdfNull : PdfObject
{
    public static readonly PdfNull Null = new();

    private PdfNull()
        : base(PdfObjectType.Null)
    {
    }

    internal static PdfNull Parse(Parsing.Lexer lexer)
    {
        lexer.Expects("null");
        return Null;
    }

    public override string ToString()
    {
        return "null";
    }
}
