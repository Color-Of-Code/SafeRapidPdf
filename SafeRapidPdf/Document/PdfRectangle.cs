using System.Drawing;
using System.IO;
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
                throw new InvalidDataException("A rectangle must have 4 values!");
            }

            Llx = (PdfNumeric)box.Items[0];
            Lly = (PdfNumeric)box.Items[1];
            Urx = (PdfNumeric)box.Items[2];
            Ury = (PdfNumeric)box.Items[3];
        }

        public PdfNumeric Llx { get; } // lower left x

        public PdfNumeric Lly { get; } // lower left y

        public PdfNumeric Urx { get; } // upper right x

        public PdfNumeric Ury { get; } // upper right y

        public double X => Llx;

        public double Y => Lly;

        public double Width => Urx - Llx;

        public double Height => Ury - Lly;

        public RectangleF ToPixels()
        {
            // NOTE: PDF dimensions are in points (1/72 in)
            const double ptToPxRatio = 4.0d / 3.0d; // 1.333

            return new RectangleF(
                x: (float)(X * ptToPxRatio),
                y: (float)(Y * ptToPxRatio),
                width: (float)(Width * ptToPxRatio),
                height: (float)(Height * ptToPxRatio)
            );
        }

        public override string ToString()
        {
            return $"{ObjectType} [{Llx}; {Lly}; {Urx}; {Ury}]";
        }
    }
}
