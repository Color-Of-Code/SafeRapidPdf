using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using SafeRapidPdf.Primitives;

namespace SafeRapidPdf.Pdf 
{
	public class PdfCatalog : PdfBaseObject
	{
		public PdfCatalog(PdfDictionary catalog)
			: base(PdfObjectType.Catalog)
		{
			IsContainer = true;
			catalog.ExpectsType("Catalog");

			PdfIndirectReference pagesRef = catalog["Pages"] as PdfIndirectReference;
			PdfDictionary pages = pagesRef.Dereference<PdfDictionary>();
			Pages = new PdfPageTree(pages, true);
		}

		public PdfPageTree Pages { get; private set; }

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				var list = new List<IPdfObject>();
				list.Add(Pages);
				return list.AsReadOnly();
			}
		}

		public override string ToString ()
		{
			return "/";
		}
	}
}
