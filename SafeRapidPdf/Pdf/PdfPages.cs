using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using SafeRapidPdf.Primitives;

namespace SafeRapidPdf.Pdf 
{
	public class PdfPages : PdfBaseObject
	{
		public PdfPages(PdfDictionary pages)
			: base(PdfObjectType.Pages)
		{
			IsContainer = true;
			pages.ExpectsType("Pages");

		}

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				var list = new List<IPdfObject>();
				//list.Add(Pages);
				return list.AsReadOnly();
			}
		}

		public override string ToString ()
		{
			return "Pages";
		}
	}
}
