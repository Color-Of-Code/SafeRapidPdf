using System;
using System.Collections.Generic;
using SafeRapidPdf.Attributes;
using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
    public class PdfPageTree : PdfPage
	{
		public PdfPageTree(PdfIndirectReference pages)
			: this(pages, null)
		{
		}

		public PdfPageTree(PdfIndirectReference pages, PdfPageTree parent)
			: base(pages, parent, PdfObjectType.PageTree)
		{
			IsContainer = true;
			var pagetree = pages.Dereference<PdfDictionary>();
			pagetree.ExpectsType("Pages");

			foreach (PdfKeyValuePair pair in pagetree.Items)
			{
				switch (pair.Key.Text)
				{
				case "Type": // skip Type Pages
					break;
				case "Kids":
					PdfArray kids = pair.Value as PdfArray;
					Kids = new List<IPdfObject>();
					foreach (PdfIndirectReference item in kids.Items)
					{
						var dic = item.Dereference<PdfDictionary>();
						String type = dic["Type"].Text;
						if (type == "Pages")
							Kids.Add(new PdfPageTree(item, this));
						else if (type == "Page")
							Kids.Add(new PdfPage(item, this));
						else
							throw new Exception("Content of Kids in a Page Tree Node must be either a Page or another Page Tree Node");
					}
					break;
				case "Count":
					Count = new PdfCount(pair.Value as PdfNumeric);
					_items.Add(Count);
					break;
				default:
					HandleKeyValuePair(pair);
					break;
				}
			}
			_items.AddRange(Kids);
		}

		[ParameterType(required:true, inheritable:false)]
		private List<IPdfObject> Kids { get; set; }

		[ParameterType(required:true, inheritable:false)]
		public PdfCount Count { get; private set; }

		public override string ToString ()
		{
            return $"Page Tree Node ({Count} kids)";
		}
	}
}
