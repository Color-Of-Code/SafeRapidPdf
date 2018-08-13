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

        public int ObjectNumber { get; }

        public int GenerationNumber { get; }

        internal IIndirectReferenceResolver Resolver { get; set; }

        public PdfIndirectObject ReferencedObject
        {
            get => Resolver.GetObject(ObjectNumber, GenerationNumber);
        }

        public T Dereference<T>()
            where T : class
        {
            return ReferencedObject.Object as T;
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

        public override string ToString() => $"{ObjectNumber} {GenerationNumber} R";
    }
}