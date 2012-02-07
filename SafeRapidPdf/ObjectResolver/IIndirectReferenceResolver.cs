using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SafeRapidPdf.File;

namespace SafeRapidPdf
{
	public interface IIndirectReferenceResolver
	{
		PdfIndirectObject GetObject(int objectNumber, int generationNumber);
	}
}
