using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using SafeRapidPdf.Primitives;

namespace SafeRapidPdf.Pdf
{
	public class Document
	{
		public Document(string version, ReadOnlyCollection<PdfObject> objects)
		{
			Version = version;
			Objects = objects;
		}

		public String Version { get; private set; }

		public ReadOnlyCollection<PdfObject> Objects { get; private set; }
	}
}
