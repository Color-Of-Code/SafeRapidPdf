using System.Collections.Generic;
using System.Linq;

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
        private PdfCatalog _root;

        public PdfDocument(PdfFile file)
            : base(PdfObjectType.Document)
        {
            _file = file;
            IsContainer = true;
        }

        public PdfCatalog Root
        {
            get
            {
                if (_root == null)
                {
                    var trailers = _file.Items.OfType<PdfTrailer>();

                    // this could happen for linearized documents
                    // if (trailers.Count() > 1)
                    //    throw new Exception("too many trailers found");

                    PdfTrailer trailer = trailers.First();
                    PdfIndirectReference root = trailer["Root"] as PdfIndirectReference;
                    PdfDictionary dic = root.Dereference<PdfDictionary>();
                    _root = new PdfCatalog(dic);
                }
                return _root;
            }
        }

        public override IReadOnlyList<IPdfObject> Items => new[] { Root };

        public IEnumerable<PdfPage> GetPages()
        {
            return GetPages(new[] { Root });
        }

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

        public override string ToString() => "Document";
    }
}