using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using SafeRapidPdf.File;
using SafeRapidPdf.Attributes;

namespace SafeRapidPdf.Document 
{
	public class PdfPageTree : PdfPage
	{
		public PdfPageTree(PdfDictionary pages)
			: this(pages, false)
		{
		}

		public PdfPageTree(PdfDictionary pages, Boolean isRoot)
			: base(pages, PdfObjectType.PageTree)
		{
			IsContainer = true;
			pages.ExpectsType("Pages");

			foreach (PdfKeyValuePair pair in pages.Items)
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
					HandleKeyValuePair(pair);
					break;
				}
			}
			_items.AddRange(Kids);
		}

		// excepted in root node
		[ParameterType(required:true, inheritable:false)]
		public IPdfObject Parent { get; private set; }

		[ParameterType(required:true, inheritable:false)]
		private List<IPdfObject> Kids { get; set; }

		[ParameterType(required:true, inheritable:false)]
		public PdfNumeric Count { get; private set; }

		public override string ToString ()
		{
			return String.Format("Page Tree Node ({0} kids)", Count);
		}
	}
}
