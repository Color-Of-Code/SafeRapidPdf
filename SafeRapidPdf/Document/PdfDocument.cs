using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
	/// <summary>
	/// Represents the document structure of a PDF document. It uses the low-
	/// level physical structure to extract the document objects.
	/// </summary>
	public class PdfDocument : PdfBaseObject
	{
		public PdfDocument(PdfFile file)
			: base(PdfObjectType.Document)
		{
			_file = file;
			IsContainer = true;
			var trailers = _file.Items.OfType<PdfTrailer>();
			// this could happen for linearized documents
			//if (trailers.Count() > 1)
			//    throw new Exception("too many trailers found");
			PdfTrailer trailer = trailers.First();
			PdfIndirectReference root =  trailer.Content["Root"] as PdfIndirectReference;
			PdfDictionary dic = root.Dereference<PdfDictionary>();
			Root = new PdfCatalog(dic);
		}

		private PdfFile _file;

		public PdfCatalog Root { get; private set; }

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				var list = new List<IPdfObject>();
				list.Add(Root);
				return list.AsReadOnly();
			}
		}

		public override string ToString ()
		{
			return "Document";
		}
	}
}
