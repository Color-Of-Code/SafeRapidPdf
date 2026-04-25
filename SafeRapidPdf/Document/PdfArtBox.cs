using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document;

/// <summary>
/// Extent of the page’s meaningful content
/// </summary>
public sealed class PdfArtBox(PdfArray box) : PdfRectangle(PdfObjectType.ArtBox, box)
{
}
