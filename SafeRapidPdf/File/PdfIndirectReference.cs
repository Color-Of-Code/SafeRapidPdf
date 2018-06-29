namespace SafeRapidPdf.File
{
    /// <summary>
    /// Immutable type
    /// </summary>
    public sealed class PdfIndirectReference : PdfObject
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
			return Parse(lexer, objectNumber);
		}

		public static PdfIndirectReference Parse(Lexical.ILexer lexer, int objectNumber)
		{
			int generationNumber = int.Parse(lexer.ReadToken());
			lexer.Expects("R");
			return new PdfIndirectReference(objectNumber, generationNumber);
		}

		public int ObjectNumber { get; }

        public int GenerationNumber { get; }

		internal IIndirectReferenceResolver Resolver { get; set;}

		public PdfIndirectObject ReferencedObject
		{
            get => Resolver.GetObject(ObjectNumber, GenerationNumber);
        }

		public override string ToString()
		{
            return $"{ObjectNumber} {GenerationNumber} R";
		}
	}
}
