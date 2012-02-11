using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
	public class PdfContents : PdfBaseObject
	{
		public PdfContents(IPdfObject obj)
			: base(PdfObjectType.Contents)
		{
			IsContainer = true;
			if (obj is PdfIndirectReference)
			{
				obj = (obj as PdfIndirectReference).ReferencedObject.Object;
			}
			PdfArray array = obj as PdfArray;
			PdfStream stream = obj as PdfStream;
			if (array != null)
			{
				Streams = array.Items;
			}
			else
			{
				if (stream != null)
				{
					var list = new List<IPdfObject>();
					list.Add(stream);
					Streams = list.AsReadOnly();
				}
				else
				{
					throw new Exception("Contents must be either a stream or an array of streams");
				}
			}
		}

		public ReadOnlyCollection<IPdfObject> Streams
		{
			get;
			private set;
		}

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				return Streams;
			}
		}

		public override string ToString()
		{
			return "Contents";
		}
	}
}
