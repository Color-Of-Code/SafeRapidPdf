using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document;

/// <summary>
/// intended dimensions of the finished page after trimming
/// </summary>
public sealed class PdfTrimBox(PdfArray box) : PdfRectangle(PdfObjectType.TrimBox, box)
{
}
