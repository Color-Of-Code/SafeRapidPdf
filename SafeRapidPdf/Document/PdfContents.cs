using System;
using System.Collections.Generic;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
    public class PdfContents : PdfBaseObject
    {
        public PdfContents(IPdfObject obj)
            : base(PdfObjectType.Contents)
        {
            IsContainer = true;

            if (obj is PdfIndirectReference reference)
            {
                obj = reference.ReferencedObject.Object;
            }
            else if (obj is PdfArray array)
            {
                Streams = array.Items;
            }
            else if (obj is PdfStream stream)
            {
                Streams = new[] { stream };
            }
            else
            {
                throw new Exception("Contents must be either a stream or an array of streams");
            }
        }

        public IReadOnlyList<IPdfObject> Streams { get; }

        public override IReadOnlyList<IPdfObject> Items => Streams;

        public override string ToString() => "Contents";
    }
}
