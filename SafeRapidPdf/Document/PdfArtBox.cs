using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document
{
    /// <summary>
    /// Extent of the page’s meaningful content
    /// </summary>
    public sealed class PdfArtBox : PdfRectangle
    {
        public PdfArtBox(PdfArray box)
            : base(PdfObjectType.ArtBox, box)
        {
        }
    }
}