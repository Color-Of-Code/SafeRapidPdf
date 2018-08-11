using System.Collections.Generic;

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

        public override string ToString() => Key.Text;

        public override IReadOnlyList<IPdfObject> Items
        {
            get => new[] { Value };
        }
    }
}