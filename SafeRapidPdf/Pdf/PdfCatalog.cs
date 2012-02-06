using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SafeRapidPdf.Primitives;

namespace SafeRapidPdf.Pdf 
{
	public class PdfCatalog : PdfBaseObject
	{
		public PdfCatalog(PdfIndirectReference root)
			: base(PdfObjectType.Catalog)
		{

		}

		public override string ToString ()
		{
			return "/";
		}
	}
}
