using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
    /// <summary>
    /// intended dimensions of the finished page after trimming
    /// </summary>
    public class PdfTrimBox : PdfRectangle
	{
		public PdfTrimBox(PdfArray box)
			: base(PdfObjectType.TrimBox, box)
		{
		}
	}
}