using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document;

/// <summary>
/// visible region of default user space
/// </summary>
public sealed class PdfCropBox(PdfArray box) : PdfRectangle(PdfObjectType.CropBox, box)
{
}
