using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using SafeRapidPdf.Primitives;

namespace SafeRapidPdf.Pdf
{
	public class FileStructure
	{
		public FileStructure(ReadOnlyCollection<PdfObject> objects)
		{
			Objects = objects;
		}

		public String Version
		{
			get
			{
				return Objects.First().ToString();
			}
		}

		public ReadOnlyCollection<PdfObject> Objects { get; private set; }
	}
}
