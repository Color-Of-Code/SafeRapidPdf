using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Services;

public interface IIndirectReferenceResolver
{
    PdfXRef XRef { get; }

    PdfIndirectObject GetObject(int objectNumber, int generationNumber);
}
