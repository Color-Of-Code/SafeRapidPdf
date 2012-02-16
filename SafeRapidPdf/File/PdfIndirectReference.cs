using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.File
{

	/// <summary>
	/// Immutable type
	/// </summary>
    public class PdfIndirectReference : PdfObject
    {
        private PdfIndirectReference(int objectNumber, int generationNumber)
			: base(PdfObjectType.IndirectReference)
        {
			ObjectNumber = objectNumber;
			GenerationNumber = generationNumber;
		}

		public T Dereference<T>() where T: class
		{
			PdfIndirectObject obj = ReferencedObject as PdfIndirectObject;
			return obj.Object as T;
		}

		public static PdfIndirectReference Parse(Lexical.ILexer lexer)
        {
			int objectNumber = int.Parse(lexer.ReadToken());
			int generationNumber = int.Parse(lexer.ReadToken());
			lexer.Expects("R");
			return new PdfIndirectReference(objectNumber, generationNumber);
		}

		public static PdfIndirectReference Parse(Lexical.ILexer lexer, String numberNumberToken, String generationNumberToken)
		{
			int objectNumber = int.Parse(numberNumberToken);
			int generationNumber = int.Parse(generationNumberToken);
			lexer.Expects("R");
			return new PdfIndirectReference(objectNumber, generationNumber);
		}

		public int ObjectNumber
		{
			get;
			private set;
		}

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
