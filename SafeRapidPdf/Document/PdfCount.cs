
using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document
{
    public sealed class PdfCount : PdfBaseObject
    {
        public PdfCount(PdfNumeric count)
            : base(PdfObjectType.Count)
        {
            Value = count.ToInt32();
        }

        public int Value { get; }

        public override string ToString() => $"Count : {Value}";
    }
}