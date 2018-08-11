using System;

namespace SafeRapidPdf.File
{
    public sealed class PdfXRefEntry : PdfObject
    {
        private PdfXRefEntry(int objectNumber, int generationNumber, long offset, char type)
            : base(PdfObjectType.XRefEntry)
        {
            ObjectNumber = objectNumber;
            GenerationNumber = generationNumber;
            Offset = offset;
            EntryType = type;
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
            char entryType = (inuse == "f") ? 'f' : 'n';
            return new PdfXRefEntry(objectNumber, generationNumber, offset, entryType);
        }

        internal static PdfXRefEntry Parse(int objectNumber, byte[] decodedXRef, int[] sizes, int row, int bytesPerEntry)
        {
            int position = 0;
            long[] result = new long[3];
            for (int column = 0; column < 3; column++)
            {
                long v = 0;
                for (int bytes = 0; bytes < sizes[column]; bytes++)
                {
                    var b = decodedXRef[row * bytesPerEntry + position];
                    v = v * 256 + b;
                    position++;
                }
                result[column] = v;
            }
            // Meaning of types and fields within an xref stream
            // type  field
            var entryType = 'f';
            long offset = 0;
            int generationNumber = 0;
            switch (result[0])
            {
                // 0     0 = f
                //       2 -> object number of next free object
                //       3 -> generation number (if used again)
                case 0:
                    entryType = 'f';
                    offset = result[1];
                    generationNumber = (int)result[2];
                    break;
                // 1     1 = n (uncompressed)
                //       2 -> byte offset in file
                //       3 -> generation number
                case 1:
                    entryType = 'n';
                    offset = result[1];
                    generationNumber = (int)result[2];
                    break;
                // 2     1 = n (compressed)
                //       2 -> object number where the data is stored
                //       3 -> index of object in the stream
                case 2:
                    entryType = 'o';
                    //TODO: access the file at that position and decode
                    offset = result[1]; // object
                    generationNumber = (int)result[2]; //index
                    break;
                default:
                    throw new Exception($"Invalid type numeric id inside xref item: {result[0]}");
            }
            return new PdfXRefEntry(objectNumber, generationNumber, offset, entryType);
        }

        public int ObjectNumber { get; }

        public int GenerationNumber { get; }

        // 'f': free (deleted objects)
        // 'n': in use
        // 'o': in use (compressed in stream)
        public bool InUse => EntryType != 'f';

        public char EntryType { get; private set; }

        public long Offset { get; }

        public override string ToString()
        {
            return $"{Offset:0000000000} {GenerationNumber:00000} {EntryType}";
        }

    }
}