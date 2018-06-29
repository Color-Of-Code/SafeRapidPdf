using System;

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

		public decimal llx { get; }
		public decimal lly { get; }
		public decimal urx { get; }
		public decimal ury { get; }

		public override string ToString ()
		{
            return $"{ObjectType} [{llx}; {lly}; {urx}; {ury}]";
		}
	}
}