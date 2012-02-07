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
			IsContainer = true;
			pages.ExpectsType("Page");
			_pages = pages;
		}

		private PdfDictionary _pages;

		public override System.Collections.ObjectModel.ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				return _pages.Items;
			}
		}

		public override string ToString ()
		{
			return "Page";
		}
	}
}
