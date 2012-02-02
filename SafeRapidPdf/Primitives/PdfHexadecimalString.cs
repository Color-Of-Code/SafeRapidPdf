using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfHexadecimalString : PdfString
	{
		public PdfHexadecimalString(IFileStructureParser parser)
		{
			String token = parser.ReadToken();
			Object = token;
			token = parser.ReadToken();
			if (token != ">")
				throw new Exception("Parser error: expected > tag");
		}

		public override string ToString()
		{
			return String.Format("<{0}>", Object);
		}
	}
}
