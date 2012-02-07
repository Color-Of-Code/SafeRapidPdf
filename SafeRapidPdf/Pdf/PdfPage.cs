using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SafeRapidPdf.Primitives;

namespace SafeRapidPdf.Pdf
{
	public class PdfPage : PdfBaseObject
	{
		public PdfPage(PdfDictionary pages)
			: base(PdfObjectType.Page)
		{
			IsContainer = false;
			pages.ExpectsType("Page");
		}

		public override string ToString ()
		{
			return "Page";
		}
	}
}
