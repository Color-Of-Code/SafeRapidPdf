using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfName : PdfObject
	{
		public PdfName(IPdfParser parser)
		{
			Object = parser.ReadToken();
		}

		public string Text { get { return (string)Object; } }

		public override string ToString()
		{
			return String.Format("/{0}", Text);
		}
	}
}
