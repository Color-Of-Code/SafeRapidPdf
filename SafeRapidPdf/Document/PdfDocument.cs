using System;
using System.Collections.Generic;

using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document
{
    /// <summary>
    /// Represents the document structure of a PDF document. It uses the low-
    /// level physical structure to extract the document objects.
    /// </summary>
    public sealed class PdfDocument : PdfBaseObject
    {
        private readonly PdfFile _file;
        private readonly PdfCatalog _root;

        public PdfDocument(PdfFile file)
            : base(PdfObjectType.Document)
        {
            _file = file;
            IsContainer = true;

            foreach (var item in _file.Items)
            {
                if (item is PdfTrailer trailer)
                {
                    var root = (PdfIndirectReference)trailer["Root"];

                    _root = new PdfCatalog(root.Dereference<PdfDictionary>());

                    break;
                }
            }

            // NOTE: Linearized documents may have multiple trailers. We use the first.

            if (_root == null)
            {
                throw new Exception("Missing trailer");
            }
        }

        public PdfCatalog Root => _root;

        public override IReadOnlyList<IPdfObject> Items => new[] { Root };

        public IEnumerable<PdfPage> GetPages()
        {
            return GetPages(_root.Items);
        }

        public override string ToString() => "Document";

        private IEnumerable<PdfPage> GetPages(IReadOnlyList<IPdfObject> objects)
        {
            if (objects != null)
            {
                foreach (var o in objects)
                {

                    if (o.ObjectType == PdfObjectType.Page)
                    {
                        yield return (PdfPage)o;
                    }

                    if (o.IsContainer && o.Items != null)
                    {
                        foreach (var page in GetPages(o.Items))
                        {
                            yield return page;
                        }
                    }

                }
            }
        }
    }
}