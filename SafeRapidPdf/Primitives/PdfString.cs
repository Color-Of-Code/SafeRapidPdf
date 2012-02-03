using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfString : PdfObject
	{
		protected String _text;

		public override string ToString()
		{
			return _text;
		}
	}
}
