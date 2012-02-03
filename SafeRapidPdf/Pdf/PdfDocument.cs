using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Pdf
{
	public class PdfDocument 
	{
		public PdfDocument(PdfFile file)
		{
			_file = file;
		}

		private PdfFile _file;
	}
}
