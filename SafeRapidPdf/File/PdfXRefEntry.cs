using System;

namespace SafeRapidPdf.File
{
    public sealed class PdfXRefEntry : PdfObject
    {
        private PdfXRefEntry(int objectNumber, int generationNumber, long offset, bool inUse)
            : base(PdfObjectType.XRefEntry)
        {
            ObjectNumber = objectNumber;
            GenerationNumber = generationNumber;
            Offset = offset;
            InUse = inUse;
        }

        public static PdfXRefEntry Parse(int objectNumber, Lexical.ILexer lexer)
        {
            string offsetS = lexer.ReadToken();
            if (offsetS.Length != 10)
                throw new Exception("Parser error: 10 digits expected for offset in xref");
            long offset = long.Parse(offsetS);

            string generationS = lexer.ReadToken();
            if (generationS.Length != 5)
                throw new Exception("Parser error: 5 digits expected for generation in xref");
            int generationNumber = int.Parse(generationS);

            string inuse = lexer.ReadToken();
            if (inuse != "f" && inuse != "n")
                throw new Exception("Parser error: only 'f' and 'n' are valid flags in xref");
            bool inUse = (inuse == "n");
            return new PdfXRefEntry(objectNumber, generationNumber, offset, inUse);
        }

        public int ObjectNumber { get; }

        public int GenerationNumber { get; }

        // 'f': free (deleted objects)
		// 'n': in use
        public bool InUse { get; }

        public long Offset { get; }

        public override string ToString()
        {
            return string.Format("{0:0000000000} {1:00000} {2}", Offset, GenerationNumber, InUse ? "n" : "f");
        }
    }
}