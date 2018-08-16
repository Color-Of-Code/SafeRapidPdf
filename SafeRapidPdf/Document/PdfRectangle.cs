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

            Llx = (PdfNumeric)box.Items[0];
            Lly = (PdfNumeric)box.Items[1];
            Urx = (PdfNumeric)box.Items[2];
            Ury = (PdfNumeric)box.Items[3];
        }

        public double Llx { get; } // lower left x

        public double Lly { get; } // lower left y

        public double Urx { get; } // upper right x

        public double Ury { get; } // upper right y

        public override string ToString() => $"{ObjectType} [{Llx}; {Lly}; {Urx}; {Ury}]";
    }
}