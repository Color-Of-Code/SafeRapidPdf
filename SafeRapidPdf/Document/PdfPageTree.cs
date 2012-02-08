using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document 
{
	public class PdfPageTree : PdfBaseObject
	{
		public PdfPageTree(PdfDictionary pages)
			: this(pages, false)
		{
		}

		public PdfPageTree(PdfDictionary pages, Boolean isRoot)
			: base(PdfObjectType.PageTree)
		{
			IsContainer = true;
			pages.ExpectsType("Pages");

			_items = new List<IPdfObject>();
			foreach (PdfKeyValuePair pair in pages.Items)
			{
				switch (pair.Key.Text)
				{
				case "Kids":
					PdfArray kids = pair.Value as PdfArray;
					Kids = new List<IPdfObject>();
					foreach (PdfIndirectReference item in kids.Items)
					{
						var dic = item.Dereference<PdfDictionary>();
						String type = dic["Type"].Text;
						if (type == "Pages")
							Kids.Add(new PdfPageTree(dic));
						else if (type == "Page")
							Kids.Add(new PdfPage(dic));
						else
							throw new Exception("Content of Kids in a Page Tree Node must be either a Page or another Page Tree Node");
					}
					break;
				case "Count":
					Count = pair.Value as PdfNumeric;
					_items.Add(pair);
					break;
				case "Parent":
					Parent = pair.Value;
					_items.Add(pair);
					break;
				default:
					_items.Add(pair);
					break;
				}
			}
		}

		private List<IPdfObject> _items;

		private IPdfObject Parent { get; set; }

		private List<IPdfObject> Kids { get; set; }

		private PdfNumeric Count { get; set; }

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				var list = new List<IPdfObject>();
				list.AddRange(_items);
				list.AddRange(Kids);
				return list.AsReadOnly();
			}
		}

		public override string ToString ()
		{
			return String.Format("Page Tree Node ({0} kids)", Count);
		}
	}
}
