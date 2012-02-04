using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SafeRapidPdf.Primitives;

namespace SafeRapidPdf
{
	public interface IIndirectReferenceResolver
	{
		PdfIndirectObject GetObject(int objectNumber, int generationNumber);
	}
}
