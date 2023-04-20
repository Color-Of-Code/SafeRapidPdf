using SafeRapidPdf.Parsing;

namespace SafeRapidPdf.Objects;

public sealed class PdfArray : PdfObject
{
    private readonly List<IPdfObject> _items;

    private PdfArray(List<IPdfObject> items)
        : base(PdfObjectType.Array)
    {
        IsContainer = true;
        _items = items;
    }

    public override IReadOnlyList<IPdfObject> Items => _items;

    public static PdfArray Parse(Lexer lexer)
    {
        ArgumentNullException.ThrowIfNull(lexer);

        var list = new List<IPdfObject>();
        PdfObject value;
        while ((value = ParseAny(lexer, "]")) != null)
        {
            list.Add(value);
        }
        return new PdfArray(list);
    }

    public override string ToString()
    {
        return "[...]";
    }
}
