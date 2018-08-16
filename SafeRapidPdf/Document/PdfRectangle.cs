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

            Llx = ((PdfNumeric)box.Items[0]).ToDecimal();
            Lly = ((PdfNumeric)box.Items[1]).ToDecimal();
            Urx = ((PdfNumeric)box.Items[2]).ToDecimal();
            Ury = ((PdfNumeric)box.Items[3]).ToDecimal();
        }

        public decimal Llx { get; } // lower left x

        public decimal Lly { get; } // lower left y

        public decimal Urx { get; } // upper right x

        public decimal Ury { get; } // upper right y

        public override string ToString() => $"{ObjectType} [{Llx}; {Lly}; {Urx}; {Ury}]";
    }
}