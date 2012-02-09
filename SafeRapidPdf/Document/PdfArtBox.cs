using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
	/// <summary>
	/// extent of the page’s meaningful content
	/// </summary>
	public class PdfArtBox : PdfRectangle
	{
		public PdfArtBox(PdfArray box)
			: base(PdfObjectType.ArtBox, box)
		{
		}
	}
}
