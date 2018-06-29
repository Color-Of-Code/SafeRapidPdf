using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
    /// <summary>
    /// extent of the page’s meaningful content
    /// </summary>
    public sealed class PdfArtBox : PdfRectangle
	{
		public PdfArtBox(PdfArray box)
			: base(PdfObjectType.ArtBox, box)
		{
		}
	}
}
