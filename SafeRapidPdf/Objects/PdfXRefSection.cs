using System.Collections.Generic;
using System.Text;
using SafeRapidPdf.Parsing;

namespace SafeRapidPdf.Objects
{
    public sealed class PdfXRefSection : PdfObject
    {
        private readonly IPdfObject[] _entries;

        private PdfXRefSection(int firstId, int size, IPdfObject[] entries)
            : base(PdfObjectType.XRefSection)
        {
            IsContainer = true;

            FirstId = firstId;
            Size = size;
            _entries = entries;
        }

        public int FirstId { get; }

        public int Size { get; }

        public override IReadOnlyList<IPdfObject> Items => _entries;

        public static PdfXRefSection Parse(PdfStream pdfStream)
        {
            var dictionary = pdfStream.StreamDictionary;
            var type = dictionary["Type"] as PdfName;
            if (type.Name != "XRef")
            {
                throw new ParsingException("A stream of type XRef is expected");
            }

            // W[1 2 1] (4 columns)
            // W[1 3 1] (5 columns, larger indexes)
            var w = dictionary["W"] as PdfArray;
            int firstId = 0;
            int size = 0;

            if (dictionary.TryGetValue("Index", out IPdfObject indexObject))
            {
                var index = (PdfArray)indexObject;
                firstId = ((PdfNumeric)index.Items[0]).ToInt32();
                size = ((PdfNumeric)index.Items[1]).ToInt32();
            }
            else if (dictionary.TryGetValue("Size", out IPdfObject sizeObject))
            {
                size = ((PdfNumeric)sizeObject).ToInt32();
            }

            int items = w.Items.Count;

            // for xref this shall always be 3
            if (items != 3)
            {
                throw new ParsingException("The W[] parameter must contain 3 columns for an XRef");
            }
            int[] sizes = new int[w.Items.Count];
            int bytesPerEntry = 0;
            for (int i = 0; i < items; i++)
            {
                sizes[i] = ((PdfNumeric)w.Items[i]).ToInt32();
                bytesPerEntry += sizes[i];
            }
            var decodedXRef = pdfStream.Decode();
            // Use W[...] to build up the xref
            int rowCount = decodedXRef.Length / bytesPerEntry;
            if (size != rowCount)
            {
                throw new ParsingException("The number of refs inside the Index value must match the actual refs count present in the stream");
            }

            var entries = new IPdfObject[rowCount];

            for (int row = 0; row < rowCount; row++)
            {
                var entry = PdfXRefEntry.Parse(firstId + row, decodedXRef, sizes, row, bytesPerEntry);
                entries[row] = entry;
            }

            return new PdfXRefSection(firstId, size, entries);
        }

        public static PdfXRefSection Parse(ILexer lexer)
        {
            int firstId = int.Parse(lexer.ReadToken());
            int size = int.Parse(lexer.ReadToken());

            var entries = new IPdfObject[size];

            for (int i = 0; i < size; i++)
            {
                var entry = PdfXRefEntry.Parse(firstId + i, lexer);

                // first entry must be free and have a gen 65535
                // head of the linked list of free objects

                if (i == 0)
                {
                    if (entry.GenerationNumber != 65535)
                    {
                        throw new ParsingException($"The first xref entry must have generation number 65535. Was {entry.GenerationNumber}");
                    }

                    if (entry.InUse)
                    {
                        throw new ParsingException($"The first xref entry must be free");
                    }
                }

                entries[i] = entry;
            }

            return new PdfXRefSection(firstId, size, entries);
        }

        public override string ToString() => $"{FirstId} {Size}";
    }
}