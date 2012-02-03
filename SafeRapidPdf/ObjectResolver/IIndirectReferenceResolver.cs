using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf
{
	public interface IIndirectReferenceResolver
	{
		void InsertObject(Primitives.PdfIndirectObject obj);

		Primitives.PdfIndirectObject GetObject(int objNumber, int genNumber);
	}
}
