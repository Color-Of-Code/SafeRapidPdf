using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
	public class PdfPage : PdfBaseObject
	{
		public PdfPage(PdfDictionary pages)
			: base(PdfObjectType.Page)
		{
			IsContainer = true;
			pages.ExpectsType("Page");

			_items = new List<IPdfObject>();
			foreach (PdfKeyValuePair pair in pages.Items)
			{
				switch (pair.Key.Text)
				{
				case "Type": // skip type Page
					break;
				case "ArtBox":
					ArtBox = new PdfArtBox(pair.Value as PdfArray);
					_items.Add(ArtBox);
					break;
				case "BleedBox":
					BleedBox = new PdfBleedBox(pair.Value as PdfArray);
					_items.Add(BleedBox);
					break;
				case "CropBox":
					CropBox = new PdfCropBox(pair.Value as PdfArray);
					_items.Add(CropBox);
					break;
				case "MediaBox":
					MediaBox = new PdfMediaBox(pair.Value as PdfArray);
					_items.Add(MediaBox);
					break;
				case "TrimBox":
					TrimBox = new PdfTrimBox(pair.Value as PdfArray);
					_items.Add(TrimBox);
					break;
				default:
					_items.Add(pair);
					break;
				}
			}
		}

		private List<IPdfObject> _items;

		//public PdfPageTree Parent { get; private set; }
		//public PdfDate LastModified { get; private set; }
		public PdfDictionary Resources { get; private set; }
		public PdfMediaBox MediaBox { get; private set; }
		public PdfCropBox CropBox { get; private set; }
		public PdfBleedBox BleedBox { get; private set; }
		public PdfTrimBox TrimBox { get; private set; }
		public PdfArtBox ArtBox { get; private set; }
		//public PdfDictionary BoxColorInfo { get; private set; }
		//public PdfStream/PdfArray Contents { get; private set; }
		public PdfNumeric Rotate { get; private set; }
		//public PdfDictionary Group { get; private set; }
		//public PdfStream Thumb { get; private set; }
		//public PdfArray B { get; private set; }
		//public PdfNumeric Dur { get; private set; }
		//public PdfDictionary Trans { get; private set; }
		//public PdfArray Annots { get; private set; }
		//public PdfDictionary AA { get; private set; }
		//public PdfStream Metadata { get; private set; }
		//public PdfDictionary PieceInfo { get; private set; }
		//public PdfNumeric StructParents { get; private set; }
		//public PdfStream ID { get; private set; }
		//public PdfNumeric PZ { get; private set; }
		//public PdfDictionary SeparationInfo { get; private set; }
		//public PdfName Tabs { get; private set; }
		//public PdfName TemplateInstantiated { get; private set; }
		//public PdfDictionary PresSteps { get; private set; }
		//public PdfNumeric UserUnit { get; private set; }
		//public PdfDictionary VP { get; private set; }

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				return _items.AsReadOnly();
			}
		}

		public override string ToString ()
		{
			return "Page";
		}
	}
}
