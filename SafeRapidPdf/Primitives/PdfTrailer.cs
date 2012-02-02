using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfTrailer : PdfObject
	{
		public PdfTrailer(IFileStructureParser parser)
		{
			Object = parser.ReadPdfObject();
		}
	}
}
