using System.Collections.Generic;

namespace SafeRapidPdf.Objects
{
    public sealed class PdfArray : PdfObject
    {
        private readonly List<IPdfObject> _items;

        private PdfArray(List<IPdfObject> items)
            : base(PdfObjectType.Array)
        {
            IsContainer = true;
            _items = items;
        }

        public override IReadOnlyList<IPdfObject> Items => _items;

        public static PdfArray Parse(Parsing.ILexer lexer)
        {
            var list = new List<IPdfObject>();
            PdfObject value;
            while ((value = PdfObject.ParseAny(lexer, "]")) != null)
            {
                list.Add(value);
            }
            return new PdfArray(list);
        }


        public override string ToString() => "[...]";
    }
}