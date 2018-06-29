using System;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
    public class PdfCount : PdfBaseObject
	{
		public PdfCount(PdfNumeric count)
			: base(PdfObjectType.Count)
		{
			Value = Convert.ToInt32(count.Value);
		}

		public int Value
		{
			get;
			private set;
		}

		public override string ToString()
		{
            return "Count : {Value}";
		}
	}
}
