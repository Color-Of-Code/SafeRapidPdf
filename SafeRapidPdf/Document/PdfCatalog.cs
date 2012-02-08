﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document 
{
	public class PdfCatalog : PdfBaseObject
	{
		public PdfCatalog(PdfDictionary catalog)
			: base(PdfObjectType.Catalog)
		{
			IsContainer = true;
			catalog.ExpectsType("Catalog");

			_items = new List<IPdfObject>();
			foreach (PdfKeyValuePair pair in catalog.Items)
			{
				if (pair.Key.Text == "Pages")
				{
					PdfDictionary pages = catalog.Resolve<PdfDictionary>("Pages");
					Pages = new PdfPageTree(pages, true);
				}
				else
				{
					_items.Add(pair);
				}
			}
		}

		private List<IPdfObject> _items;
		public PdfPageTree Pages { get; private set; }

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				var list = new List<IPdfObject>();
				list.AddRange(_items);
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
