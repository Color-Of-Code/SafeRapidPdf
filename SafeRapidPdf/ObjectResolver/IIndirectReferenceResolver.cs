using SafeRapidPdf.File;

namespace SafeRapidPdf
{
    public interface IIndirectReferenceResolver
	{
		PdfXRef XRef { get; }
		
		PdfIndirectObject GetObject(int objectNumber, int generationNumber);
	}
}