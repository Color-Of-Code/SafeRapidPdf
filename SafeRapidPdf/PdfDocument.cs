using System.Collections.Generic;
using System.IO;
using SafeRapidPdf.Document;
using SafeRapidPdf.Objects;

namespace SafeRapidPdf
{
    /// <summary>
    /// Represents the document structure of a PDF document.
    /// </summary>
    public class PdfDocument
    {
        private readonly PdfFile _file;

        public PdfDocument(PdfFile file)
        {
            _file = file;

            foreach (var item in _file.Items)
            {
                if (item is PdfTrailer trailer)
                {
                    var root = (PdfIndirectReference)trailer["Root"];

                    Root = new PdfCatalog(root.Dereference<PdfDictionary>());

                    break;
                }
            }

            // NOTE: Linearized documents may have multiple trailers. We use the first.

            if (Root is null)
            {
                throw new InvalidDataException("Missing trailer");
            }
        }

        public PdfCatalog Root { get; }

        public static PdfDocument Load(Stream stream)
        {
            return new PdfDocument(PdfFile.Parse(stream));
        }

        public IEnumerable<PdfPage> GetPages()
        {
            return GetPages(Root.Items);
        }

        public override string ToString()
        {
            return "Document";
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
    }
}
