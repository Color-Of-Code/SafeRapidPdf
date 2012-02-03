using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{

    public class PdfIndirectObject : PdfObject
    {
        public PdfIndirectObject(Lexical.ILexer lexer)
        {
			IsContainer = true;

			ObjectNumber = int.Parse(lexer.ReadToken());
			GenerationNumber = int.Parse(lexer.ReadToken());

			lexer.Expects("obj");

			PdfObject = PdfObject.Parse(lexer);

			lexer.Expects("endobj");
		}

		public int ObjectNumber { get; private set; }

		public int GenerationNumber { get; private set; }

		public PdfObject PdfObject
		{
			get;
			private set;
		}

		public override string ToString()
		{
			return String.Format("{0} {1} obj", ObjectNumber, GenerationNumber);
		}
	}
}
