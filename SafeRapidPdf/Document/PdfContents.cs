using System.Collections.Generic;
using System.IO;
using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document
{
    public sealed class PdfContents : PdfBaseObject
    {
        public PdfContents(IPdfObject obj)
            : base(PdfObjectType.Contents)
        {
            IsContainer = true;

            if (obj is PdfIndirectReference reference)
            {
                obj = reference.ReferencedObject.Object;
            }

            Streams = obj is PdfArray array
                ? array.Items
                : obj is PdfStream stream
                    ? (new[] { stream })
                    : throw new InvalidDataException("Contents must be either a stream or an array of streams");
        }

        public IReadOnlyList<IPdfObject> Streams { get; }

        public override IReadOnlyList<IPdfObject> Items => Streams;

        public override string ToString()
        {
            return "Contents";
        }
    }
}
