using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{

	/// <summary>
	/// Immutable type
	/// </summary>
    public class PdfIndirectReference : PdfObject
    {
        private PdfIndirectReference(int objectNumber, int generationNumber, IIndirectReferenceResolver resolver)
        {
			ObjectNumber = objectNumber;
			GenerationNumber = generationNumber;
			Resolver = resolver;
		}

		public static PdfIndirectReference Parse(Lexical.ILexer lexer, IIndirectReferenceResolver resolver)
        {
			int objectNumber = int.Parse(lexer.ReadToken());
			int generationNumber = int.Parse(lexer.ReadToken());
			lexer.Expects("R");
			return new PdfIndirectReference(objectNumber, generationNumber, resolver);
		}

		public int ObjectNumber { get; private set; }

        public int GenerationNumber { get; private set; }

		internal IIndirectReferenceResolver Resolver { get; set;}

		public PdfIndirectObject ReferencedObject
		{
			get
			{
				return Resolver.GetObject(ObjectNumber, GenerationNumber);
			}
		}

		public override string ToString()
		{
			return String.Format("{0} {1} R", ObjectNumber, GenerationNumber);
		}
	}
}
