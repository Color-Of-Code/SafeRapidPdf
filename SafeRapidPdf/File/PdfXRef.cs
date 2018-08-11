using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SafeRapidPdf.File
{
    public sealed class PdfXRef : PdfObject
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
                        string key = BuildKey(entry.ObjectNumber, entry.GenerationNumber);
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
            string token = lexer.PeekToken1();
            while (token != null && char.IsDigit(token[0]))
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
            var sections = new List<PdfXRefSection>(xrefStream.Length);
            foreach (var pdfStream in xrefStream)
            {
                sections.Add(PdfXRefSection.Parse(pdfStream));
            }
            return new PdfXRef(sections);
        }

        private IList<PdfXRefSection> _sections;

        public long GetOffset(int objectNumber, int generationNumber)
        {
            string key = BuildKey(objectNumber, generationNumber);
            return _offsets[key];
        }

        public static string BuildKey(int objectNumber, int generationNumber)
        {
            return $"{objectNumber:0000000000}_{generationNumber:00000}";
        }

        private Dictionary<string, long> _offsets = new Dictionary<string, long>();

        public override ReadOnlyCollection<IPdfObject> Items
        {
            get => _sections.ToList().ConvertAll(x => x as IPdfObject).AsReadOnly();
        }

        public override string ToString() => "xref";
    }
}
