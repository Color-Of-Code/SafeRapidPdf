using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Pdf
{
	/// <summary>
	/// Represents the logical structure of a PDF document. It uses the low-
	/// level physical structure to extract the logical objects.
	/// </summary>
	public class PdfDocument : IPdfObject
	{
		public PdfDocument(PdfFile file)
		{
			_file = file;
		}

		private PdfFile _file;

		public string Text
		{
			get { return "root"; }
		}

		public bool IsContainer
		{
			get { return true; }
		}

		public ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				return null;
			}
		}

		public PdfObjectType ObjectType
		{
			get { return PdfObjectType.Document; }
		}
	}
}
