using System;
using System.Collections.Generic;
using System.IO;
using SafeRapidPdf.Attributes;
using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document;

public sealed class PdfPageTree : PdfPage
{
    public PdfPageTree(PdfIndirectReference pages)
        : this(pages, null)
    {
    }

    public PdfPageTree(PdfIndirectReference pages, PdfPageTree parent)
        : base(pages, parent, PdfObjectType.PageTree)
    {
        if (pages is null)
        {
            throw new ArgumentNullException(nameof(pages));
        }

        IsContainer = true;
        var pageTree = pages.Dereference<PdfDictionary>();
        pageTree.ExpectsType("Pages");

        foreach (PdfKeyValuePair pair in pageTree.Items)
        {
            switch (pair.Key.Text)
            {
                case "Type": // skip Type Pages
                    break;
                case "Kids":
                    var kids = (PdfArray)pair.Value;
                    Kids = new List<IPdfObject>();
                    foreach (PdfIndirectReference item in kids.Items)
                    {
                        var dic = item.Dereference<PdfDictionary>();
                        string type = dic["Type"].Text;
                        if (type == "Pages")
                            Kids.Add(new PdfPageTree(item, this));
                        else if (type == "Page")
                            Kids.Add(new PdfPage(item, this));
                        else
                            throw new InvalidDataException("Content of Kids in a Page Tree Node must be either a Page or another Page Tree Node");
                    }
                    break;
                case "Count":
                    Count = new PdfCount(pair.Value as PdfNumeric);
                    _items.Add(Count);
                    break;
                default:
                    HandleKeyValuePair(pair);
                    break;
            }
        }
        _items.AddRange(Kids);
    }

    [ParameterType(required: true, inheritable: false)]
    public PdfCount Count { get; }

    [ParameterType(required: true, inheritable: false)]
    private List<IPdfObject> Kids { get; set; }

    public override string ToString()
    {
        return $"Page Tree Node ({Count} kids)";
    }
}
