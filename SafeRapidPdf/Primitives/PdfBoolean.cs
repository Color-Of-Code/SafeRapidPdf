using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfBoolean : PdfObject
	{
		public PdfBoolean(bool value)
		{
			Object = value;
		}
	}
}
