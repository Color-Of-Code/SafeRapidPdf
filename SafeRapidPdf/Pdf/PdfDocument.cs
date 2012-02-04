using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Pdf
{
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
	}
}
