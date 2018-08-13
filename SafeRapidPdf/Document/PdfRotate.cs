using System;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
    public sealed class PdfRotate : PdfBaseObject
    {
        internal PdfRotate(PdfNumeric value)
            : base(PdfObjectType.Rotate)
        {
            Value = Convert.ToInt32(value.Value);
        }

        public int Value { get; }

        public override string ToString() => $"Rotate {Value} degrees";
    }
}