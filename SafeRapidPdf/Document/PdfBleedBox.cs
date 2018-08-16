using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document
{
    /// <summary>
    /// region to which the contents of the page should be clipped
    /// when output in a production environment
    /// </summary>
    public sealed class PdfBleedBox : PdfRectangle
    {
        public PdfBleedBox(PdfArray box)
            : base(PdfObjectType.BleedBox, box)
        {
        }
    }
}