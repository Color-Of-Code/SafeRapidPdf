using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
    /// <summary>
    /// visible region of default user space
    /// </summary>
    public sealed class PdfCropBox : PdfRectangle
    {
        public PdfCropBox(PdfArray box)
            : base(PdfObjectType.CropBox, box)
        {
        }
    }
}