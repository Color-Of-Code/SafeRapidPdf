using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{

    public class PdfIndirectObject : PdfObject
    {
        public PdfIndirectObject(int objectNumber, int generationNumber, IFileStructureParser parser)
        {
			IsContainer = true;

			ObjectNumber = objectNumber;
			GenerationNumber = generationNumber;

			PdfObject refobj = parser.ReadPdfObject();
			string token = parser.ReadToken();
			if (token != "endobj")
			{
				if (token != "stream")
					throw new Exception("Only streams supported here");
				PdfObject obj2 = parser.ReadPdfObject(token);
				Object = obj2;
				token = parser.ReadToken();
				if (token != "endobj")
					throw new Exception("Parser error: expected endobj tag");
			}
			else
				Object = refobj;
		}

		public int ObjectNumber { get; private set; }

		public int GenerationNumber { get; private set; }

		public PdfObject PdfObject
        {
            get { return Object as PdfObject; }
        }

		public override string ToString()
		{
			return String.Format("{0} {1} obj", ObjectNumber, GenerationNumber);
		}
	}
}
