using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SafeRapidPdf.File
{
    /// <summary>
    /// Object not described in the specification but eases use and
    /// implementation in .NET
    /// </summary>
    public sealed class PdfKeyValuePair : PdfObject
    {
        public PdfKeyValuePair(PdfName key, PdfObject value)
            : base(PdfObjectType.KeyValuePair)
        {
            IsContainer = true;
            Key = key;
            Value = value;
        }

        public PdfName Key { get; }

        public PdfObject Value { get; }

        public override string ToString()
        {
            return Key.Text;
        }

        public override ReadOnlyCollection<IPdfObject> Items
        {
            get
            {
                var list = new List<IPdfObject>(1)
                {
                    Value
                };

                return list.AsReadOnly();
            }
        }
    }
}
