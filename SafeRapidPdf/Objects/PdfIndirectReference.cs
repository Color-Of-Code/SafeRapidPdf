using System.Globalization;
using SafeRapidPdf.Services;

namespace SafeRapidPdf.Objects;

/// <summary>
/// Immutable type
/// </summary>
public sealed class PdfIndirectReference : PdfObject
{
    private PdfIndirectReference(int objectNumber, int generationNumber)
        : base(PdfObjectType.IndirectReference)
    {
        ObjectNumber = objectNumber;
        GenerationNumber = generationNumber;
    }

    public int ObjectNumber { get; }

    public int GenerationNumber { get; }

    public PdfIndirectObject ReferencedObject
        => Resolver.GetObject(ObjectNumber, GenerationNumber);

    internal IIndirectReferenceResolver Resolver { get; set; }

    public T Dereference<T>()
        where T : class
    {
        return ReferencedObject.Object as T;
    }

    internal static PdfIndirectReference Parse(Parsing.Lexer lexer)
    {
        int objectNumber = int.Parse(lexer.ReadToken(), CultureInfo.InvariantCulture);
        return Parse(lexer, objectNumber);
    }

    internal static PdfIndirectReference Parse(Parsing.Lexer lexer, int objectNumber)
    {
        int generationNumber = int.Parse(lexer.ReadToken(), CultureInfo.InvariantCulture);
        lexer.Expects("R");
        return new PdfIndirectReference(objectNumber, generationNumber);
    }

    public override string ToString()
    {
        return $"{ObjectNumber} {GenerationNumber} R";
    }
}
