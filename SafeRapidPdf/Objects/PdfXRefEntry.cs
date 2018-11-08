using System.Globalization;
using SafeRapidPdf.Parsing;

namespace SafeRapidPdf.Objects
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

        public int ObjectNumber { get; }

        public int GenerationNumber { get; }

        public char EntryType { get; }

        public long Offset { get; }

        // 'f': free (deleted objects)
        // 'n': in use
        // 'o': in use (compressed in stream)
        public bool InUse => EntryType != 'f';

        public static PdfXRefEntry Parse(int objectNumber, Lexer lexer)
        {
            string offsetS = lexer.ReadToken();
            if (offsetS.Length != 10)
            {
                throw new ParsingException("Expected 10 digits for offset in xref");
            }
            long offset = long.Parse(offsetS, CultureInfo.InvariantCulture);

            string generationS = lexer.ReadToken();
            if (generationS.Length != 5)
            {
                throw new ParsingException("Expected 5 digits for generation in xref");
            }
            int generationNumber = int.Parse(generationS, CultureInfo.InvariantCulture);

            string inuse = lexer.ReadToken();
            if (inuse != "f" && inuse != "n")
            {
                throw new ParsingException($"xref flag must be 'f' or 'n'. Was {inuse}");
            }

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

                    // TODO: access the file at that position and decode
                    offset = result[1]; // object
                    generationNumber = (int)result[2]; // index
                    break;
                default:
                    throw new ParsingException($"Invalid type numeric id inside xref item: {result[0]}");
            }

            return new PdfXRefEntry(objectNumber, generationNumber, offset, entryType);
        }

        public override string ToString() => $"{Offset:0000000000} {GenerationNumber:00000} {EntryType}";
    }
}