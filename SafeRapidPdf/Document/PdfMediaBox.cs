using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document;

/// <summary>
/// boundaries of the physical medium on which the page is
/// intended to be displayed or printed
/// </summary>
public sealed class PdfMediaBox(PdfArray box) : PdfRectangle(PdfObjectType.MediaBox, box)
{
}
