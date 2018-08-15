using System;

using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document
{
    public abstract class PdfRectangle : PdfBaseObject
    {
        protected PdfRectangle(PdfObjectType type, PdfArray box)
            : base(type)
        {
            if (box.Items.Count != 4)
            {
                throw new Exception("A rectangle must have 4 values!");
            }

            Llx = (box.Items[0] as PdfNumeric).Value;
            Lly = (box.Items[1] as PdfNumeric).Value;
            Urx = (box.Items[2] as PdfNumeric).Value;
            Ury = (box.Items[3] as PdfNumeric).Value;
        }

        public decimal Llx { get; } // lower left x

        public decimal Lly { get; } // lower left y

        public decimal Urx { get; } // upper right x

        public decimal Ury { get; } // upper right y

        public override string ToString() => $"{ObjectType} [{Llx}; {Lly}; {Urx}; {Ury}]";
    }
}