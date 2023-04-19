using System.IO;

using SafeRapidPdf.Attributes;
using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Document;

public class PdfPage : PdfBaseObject
{
    private readonly List<IPdfObject> _items = new();

    public PdfPage(PdfIndirectReference pages, PdfPageTree parent)
        : this(pages, parent, PdfObjectType.Page)
    {
        ArgumentNullException.ThrowIfNull(pages);

        IsContainer = true;

        var page = pages.Dereference<PdfDictionary>();

        page.ExpectsType("Page");

        foreach (PdfKeyValuePair pair in page.Items)
        {
            HandleKeyValuePair(pair);
        }
    }

    protected PdfPage(PdfIndirectReference pages, PdfPageTree parent, PdfObjectType type)
        : base(type)
    {
        ArgumentNullException.ThrowIfNull(pages);

        GenerationNumber = pages.GenerationNumber;
        ObjectNumber = pages.ObjectNumber;
        Parent = parent;
    }

    protected void Add(IPdfObject item)
    {
        _items.Add(item);
    }


    protected void AddRange(IEnumerable<IPdfObject> collection)
    {
        _items.AddRange(collection);
    }

    protected int GenerationNumber { get; }

    protected int ObjectNumber { get; }

    // excepted in root node
    [ParameterType(required: true, inheritable: false)]
    public PdfPageTree Parent { get; }

    // public PdfDate LastModified { get; private set; }

    public PdfDictionary Resources { get; private set; }

    [ParameterType(required: true, inheritable: true)]
    public PdfMediaBox MediaBox { get; private set; }

    [ParameterType(required: false, inheritable: true)]
    public PdfCropBox CropBox { get; private set; }

    [ParameterType(required: false, inheritable: false, version: "1.3")]
    public PdfBleedBox BleedBox { get; private set; }

    [ParameterType(required: false, inheritable: false, version: "1.3")]
    public PdfTrimBox TrimBox { get; private set; }

    [ParameterType(required: false, inheritable: false, version: "1.3")]
    public PdfArtBox ArtBox { get; private set; }

    // public PdfDictionary BoxColorInfo { get; private set; }

    [ParameterType(required: false, inheritable: false)]
    public PdfContents Contents { get; private set; }

    [ParameterType(required: false, inheritable: true)]
    public PdfRotate Rotate { get; private set; }

    // public PdfDictionary Group { get; private set; }
    // public PdfStream Thumb { get; private set; }
    // public PdfArray B { get; private set; }
    // public PdfNumeric Dur { get; private set; }
    // public PdfDictionary Trans { get; private set; }
    // public PdfArray Annots { get; private set; }
    // public PdfDictionary AA { get; private set; }
    // public PdfStream Metadata { get; private set; }
    // public PdfDictionary PieceInfo { get; private set; }
    // public PdfNumeric StructParents { get; private set; }
    // public PdfStream ID { get; private set; }
    // public PdfNumeric PZ { get; private set; }
    // public PdfDictionary SeparationInfo { get; private set; }
    // public PdfName Tabs { get; private set; }
    // public PdfName TemplateInstantiated { get; private set; }
    // public PdfDictionary PresSteps { get; private set; }
    // public PdfNumeric UserUnit { get; private set; }
    // public PdfDictionary VP { get; private set; }

    public override IReadOnlyList<IPdfObject> Items => _items;

    protected void HandleKeyValuePair(PdfKeyValuePair pair)
    {
        ArgumentNullException.ThrowIfNull(pair);

        switch (pair.Key.Text)
        {
            case "Type": // skip type Page
                break;
            case "ArtBox":
                ArtBox = new PdfArtBox((PdfArray)pair.Value);
                _items.Add(ArtBox);
                break;
            case "BleedBox":
                BleedBox = new PdfBleedBox((PdfArray)pair.Value);
                _items.Add(BleedBox);
                break;
            case "CropBox":
                CropBox = new PdfCropBox((PdfArray)pair.Value);
                _items.Add(CropBox);
                break;
            case "MediaBox":
                MediaBox = new PdfMediaBox((PdfArray)pair.Value);
                _items.Add(MediaBox);
                break;
            case "TrimBox":
                TrimBox = new PdfTrimBox((PdfArray)pair.Value);
                _items.Add(TrimBox);
                break;
            case "Rotate":
                Rotate = new PdfRotate((PdfNumeric)pair.Value);
                _items.Add(Rotate);
                break;
            case "Contents":
                Contents = new PdfContents(pair.Value);
                _items.Add(Contents);
                break;
            case "Parent":
                var parent = (PdfIndirectReference)pair.Value;
                if (parent.ObjectNumber != Parent.ObjectNumber)
                {
                    throw new InvalidDataException("Unexpected not matching parent object number!");
                }
                if (parent.GenerationNumber != Parent.GenerationNumber)
                {
                    throw new InvalidDataException("Unexpected not matching parent generation number!");
                }
                // ignore entry (parent is shown through the hierarchy
                break;
            default:
                _items.Add(pair);
                break;
        }
    }

    public override string ToString()
    {
        return "Page";
    }
}
