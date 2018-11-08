using SafeRapidPdf.Parsing;

namespace SafeRapidPdf.Objects
{
    public sealed class PdfTrailer : PdfDictionary
    {
        private PdfTrailer(PdfDictionary dictionary)
            : base(dictionary, PdfObjectType.Trailer)
        {
        }

        public static new PdfTrailer Parse(Lexer lexer)
        {
            lexer.Expects("<<");
            var dictionary = PdfDictionary.Parse(lexer);
            return new PdfTrailer(dictionary);
        }

        public override string ToString() => "trailer";
    }
}