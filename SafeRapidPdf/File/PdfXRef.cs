using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SafeRapidPdf.File
{
    public class PdfXRef : PdfObject
    {
        private PdfXRef(IList<PdfXRefSection> sections)
            : base(PdfObjectType.XRef)
        {
            IsContainer = true;

            _sections = sections;
            // create the access table
            foreach (var section in _sections)
            {
                foreach (var entryItem in section.Items)
                {
                    PdfXRefEntry entry = entryItem as PdfXRefEntry;
                    if (entry.InUse)
                    {
                        String key = BuildKey(entry.ObjectNumber, entry.GenerationNumber);
                        _offsets.Add(key, entry.Offset);
                    }
                }
            }
        }

        /// <summary>
        /// Parse an uncompressed xref dictionary
        /// </summary>
        /// <param name="lexer"></param>
        /// <returns></returns>
		public static PdfXRef Parse(Lexical.ILexer lexer)
        {
            var sections = new List<PdfXRefSection>();
            String token = lexer.PeekToken1();
            while (token != null && Char.IsDigit(token[0]))
            {
                sections.Add(PdfXRefSection.Parse(lexer));
                token = lexer.PeekToken1();
            }
            return new PdfXRef(sections);
        }

        /// <summary>
        /// Parse the xref table out of a compressed stream
        /// </summary>
        /// <param name="xrefStream"></param>
        /// <returns></returns>
        public static PdfXRef Parse(params PdfStream[] xrefStream)
        {
            foreach (var pdfStream in xrefStream)
            {
                // W[1 2 1] (4 columns) 
                // W[1 3 1] (5 columns, larger indexes)
                var w = pdfStream.StreamDictionary["W"] as PdfArray;
                int items = w.Items.Count;
                // for xref this shall always be 3
                if (items != 3)
                    throw new Exception("The W[] parameter must contain 3 columns for an XRef");
                int[] sizes = new int[w.Items.Count];
                int bytesPerEntry = 0;
                for (int i = 0; i < items; i++)
                {
                    sizes[i] = (int)(w.Items[i] as PdfNumeric).Value;
                    bytesPerEntry += sizes[i];
                }
                var decodedXRef = pdfStream.Decode();
                // Use W[...] to build up the xref
                int rows = decodedXRef.Length / bytesPerEntry;
                for (int r = 0; r < rows; r++)
                {
                    throw new NotImplementedException("Work in Progress: Parse entries");
                    // Meaning of types and fields within an xref stream
                    // type  field
                    // 0     0 = f
                    //       2 -> object number of next free object
                    //       3 -> generation number (if used again)
                    // 1     1 = n (uncompressed)
                    //       2 -> byte offset in file
                    //       3 -> generation number
                    // 2     1 = n (compressed)
                    //       2 -> object number where the data is stored
                    //       3 -> index of object in the stream
                }
            }

            throw new NotImplementedException("Work in Progress");
        }

        private IList<PdfXRefSection> _sections;

        public long GetOffset(int objectNumber, int generationNumber)
        {
            String key = BuildKey(objectNumber, generationNumber);
            return _offsets[key];
        }

        public static String BuildKey(int objectNumber, int generationNumber)
        {
            return String.Format("{0:0000000000}_{1:00000}", objectNumber, generationNumber);
        }

        private Dictionary<String, long> _offsets = new Dictionary<string, long>();

        public override ReadOnlyCollection<IPdfObject> Items
        {
            get => _sections.ToList().ConvertAll(x => x as IPdfObject).AsReadOnly();
        }

        public override string ToString()
        {
            return "xref";
        }
    }
}
