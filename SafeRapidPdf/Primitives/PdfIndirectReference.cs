using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{

	/// <summary>
	/// Immutable type
	/// </summary>
    public class PdfIndirectReference : PdfObject
    {
        public PdfIndirectReference(Lexical.ILexer lexer)
        {
			ObjectNumber = int.Parse(lexer.ReadToken());
			GenerationNumber = int.Parse(lexer.ReadToken());
			lexer.Expects("R");
		}

		public int ObjectNumber { get; private set; }

        public int GenerationNumber { get; private set; }

		public override string ToString()
		{
			return String.Format("{0} {1} R", ObjectNumber, GenerationNumber);
		}
	}
}
