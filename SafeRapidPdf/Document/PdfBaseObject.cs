namespace SafeRapidPdf.Document;

public abstract class PdfBaseObject(PdfObjectType type) : IPdfObject
{
    public PdfObjectType ObjectType { get; } = type;

    public bool IsContainer { get; protected set; }

    public string Text => ToString();

    public virtual IReadOnlyList<IPdfObject> Items
        => !IsContainer
            ? null
            : throw new InvalidOperationException("Subclass must override Items when IsContainer is true.");
}
