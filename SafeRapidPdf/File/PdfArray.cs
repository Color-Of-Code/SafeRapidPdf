using System.Collections.Generic;

namespace SafeRapidPdf.File
{
    public sealed class PdfArray : PdfObject
    {
        private PdfArray(List<IPdfObject> items)
            : base(PdfObjectType.Array)
        {
            IsContainer = true;
            _items = items;
        }

        public static PdfArray Parse(Lexical.ILexer lexer)
        {
            var list = new List<IPdfObject>();
            PdfObject value;
            while ((value = PdfObject.ParseAny(lexer, "]")) != null)
            {
                list.Add(value);
            }
            return new PdfArray(list);
        }

        private readonly List<IPdfObject> _items;
        public override IReadOnlyList<IPdfObject> Items
        {
            get => _items.AsReadOnly();
        }

        public override string ToString()
        {
            return "[...]";
        }
    }
}