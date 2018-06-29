using System;
using System.Globalization;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
    public sealed class PdfRotate : PdfBaseObject
	{
		internal PdfRotate(PdfNumeric value)
			: base(PdfObjectType.Rotate)
		{
			Value = Convert.ToInt32(value.Value);
		}

		public int Value { get ; private set; }

		public override string ToString ()
		{
			return String.Format(CultureInfo.InvariantCulture, "Rotate {0} degrees", Value);
		}
	}
}