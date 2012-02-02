using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfNumeric : PdfObject
	{
		public PdfNumeric(string token)
		{
			Object = decimal.Parse(token, System.Globalization.CultureInfo.InvariantCulture);
		}

		public decimal Value
		{
			get
			{
				return (decimal)Object;
			}
		}

		public override string ToString()
		{
			return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}
