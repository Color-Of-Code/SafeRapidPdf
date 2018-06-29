using SafeRapidPdf.File;

namespace SafeRapidPdf
{
    public interface IIndirectReferenceResolver
	{
		PdfIndirectObject GetObject(int objectNumber, int generationNumber);
	}
}