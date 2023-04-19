using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document;

public sealed class PdfCount : PdfBaseObject
{
    public PdfCount(PdfNumeric count)
        : base(PdfObjectType.Count)
    {
        ArgumentNullException.ThrowIfNull(count);

        Value = count.ToInt32();
    }

    public int Value { get; }

    public override string ToString()
    {
        return $"Count : {Value}";
    }
}
