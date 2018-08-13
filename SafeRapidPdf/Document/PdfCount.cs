using System;

using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document
{
    public sealed class PdfCount : PdfBaseObject
    {
        public PdfCount(PdfNumeric count)
            : base(PdfObjectType.Count)
        {
            Value = Convert.ToInt32(count.Value);
        }

        public int Value { get; }

        public override string ToString() => $"Count : {Value}";
    }
}