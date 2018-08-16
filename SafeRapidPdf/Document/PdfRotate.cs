using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document
{
    public sealed class PdfRotate : PdfBaseObject
    {
        internal PdfRotate(PdfNumeric value)
            : base(PdfObjectType.Rotate)
        {
            Value = value.ToInt32();
        }

        public int Value { get; }

        public override string ToString() => $"Rotate {Value} degrees";
    }
}