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
        public static PdfXRef Parse(PdfObject xrefStream)
        {
            throw new NotImplementedException("Work in Progress");
            /*
            var sections = new List<PdfXRefSection>();
            String token = lexer.PeekToken1();
            while (Char.IsDigit(token[0]))
            {
                sections.Add(PdfXRefSection.Parse(lexer));
                token = lexer.PeekToken1();
            }
            return new PdfXRef(sections);
             */
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
