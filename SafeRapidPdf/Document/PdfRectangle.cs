using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
	public abstract class PdfRectangle : PdfBaseObject
	{
		protected PdfRectangle(PdfObjectType type, PdfArray box)
			: base(type)
		{
			if (box.Items.Count != 4)
				throw new Exception("A rectangle must have 4 values!");
			llx = (box.Items[0] as PdfNumeric).Value;
			lly = (box.Items[1] as PdfNumeric).Value;
			urx = (box.Items[2] as PdfNumeric).Value;
			ury = (box.Items[3] as PdfNumeric).Value;
		}

		public decimal llx { get; private set; }
		public decimal lly { get; private set; }
		public decimal urx { get; private set; }
		public decimal ury { get; private set; }

		public override string ToString ()
		{
			return String.Format(CultureInfo.InvariantCulture, "{4} [{0}; {1}; {2}; {3}]", llx, lly, urx, ury, ObjectType);
		}
	}
}
