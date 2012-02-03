using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf
{
	public interface IFileStructureParser
	{
		/// <summary>
		/// Parse the document
		/// </summary>
		/// <returns></returns>
		Pdf.FileStructure Parse();
	}
}
