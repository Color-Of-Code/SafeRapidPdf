using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Pdf
{
	public class Document 
	{
		public Document(FileStructure structure)
		{
			_structure = structure;
		}

		private FileStructure _structure;
	}
}
