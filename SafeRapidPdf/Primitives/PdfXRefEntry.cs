using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfXRefEntry : PdfObject
	{
		public PdfXRefEntry(int objectNumber, Lexical.ILexer lexer)
		{
			ObjectNumber = objectNumber;

			string offsetS = lexer.ReadToken();
			if (offsetS.Length != 10)
				throw new Exception("Parser error: 10 digits expected for offset in xref");
			Offset = long.Parse(offsetS);

			string generationS = lexer.ReadToken();
			if (generationS.Length != 5)
				throw new Exception("Parser error: 5 digits expected for generation in xref");
			GenerationNumber = int.Parse(generationS);

			string inuse = lexer.ReadToken();
			if (inuse != "f" && inuse != "n")
				throw new Exception("Parser error: only 'f' and 'n' are valid flags in xref");
			InUse = (inuse == "n");
		}

		public int ObjectNumber { get; private set; }
		public int GenerationNumber { get; private set; }
		public bool InUse { get; private set; }
		public long Offset { get; private set; }

		public override string ToString()
		{
			return String.Format("{0:0000000000} {1:00000} {2}", Offset, GenerationNumber, InUse ? "n" : "f");
		}
	}
}
