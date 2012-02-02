using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfStream : PdfObject
	{
		public PdfStream(PdfDictionary dictionary, IPdfParser parser)
		{
			if (dictionary == null)
				throw new Exception("Parser error: stream needs a dictionary");

			PdfObject lengthObject = dictionary["Length"];
			if (lengthObject == null)
				throw new Exception("Parser error: stream dictionary requires a Length entry");

			int length = 0;
			if (lengthObject is PdfIndirectReference)
			{
				PdfIndirectReference reference = lengthObject as PdfIndirectReference;
				PdfIndirectObject lenobj = parser.GetObject(reference.ObjectNumber, reference.GenerationNumber);
				PdfNumeric len = lenobj.Object as PdfNumeric;
				length = int.Parse(len.ToString());
			}
			else
			{
				length = int.Parse(lengthObject.ToString());
			}

			Object = parser.ReadBytes(length);
			String token = parser.ReadToken();
			if (token != "endstream")
				throw new Exception("Parser error: endstream tag expected");

			StreamDictionary = dictionary;
		}

		public PdfObject StreamDictionary
		{
			get;
			private set;
		}
	}
}
