using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfArray : PdfObject
	{
		public PdfArray(IPdfParser parser)
		{
			IsContainer = true;
			var list = new List<PdfObject>();
			String token;
			while ((token = parser.ReadToken()) != "]")
			{
				PdfObject value = parser.ReadPdfObject(token);
				list.Add(value);
			}
			Object = list;
		}

		public ReadOnlyCollection<PdfObject> Items
		{
			get { return (Object as List<PdfObject>).AsReadOnly(); }
		}

		public override string ToString()
		{
			return String.Format("[{0}]", String.Join(" ", Items.Select(x => x.ToString())));
		}
	}
}
