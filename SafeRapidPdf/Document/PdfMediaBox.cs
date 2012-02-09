using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document 
{
	/// <summary>
	/// boundaries of the physical medium on which the page is
	/// intended to be displayed or printed
	/// </summary>
	public class PdfMediaBox : PdfRectangle
	{
		public PdfMediaBox(PdfArray box)
			: base(PdfObjectType.MediaBox, box)
		{
		}
	}
}
