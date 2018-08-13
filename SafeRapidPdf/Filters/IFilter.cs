using SafeRapidPdf.Objects;

namespace SafeRapidPdf.Filters
{
    public interface IFilter
    {
        /// <summary>
        /// Decodes the input buffer and returns a decoded output buffer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        byte[] Decode(byte[] input, PdfDictionary options);
    }
}
