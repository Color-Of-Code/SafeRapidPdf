namespace SafeRapidPdf.File
{
    /// <summary>
    /// Comments start with % and end on EOL char (CR or LF)
    /// </summary>
    public sealed class PdfComment : PdfObject
    {
        private readonly string _text;

        private PdfComment(string text)
            : base(PdfObjectType.Comment)
        {
            _text = text;
        }

        public bool IsEOF => _text == "%EOF";

        public static PdfComment Parse(Lexical.ILexer lexer)
        {
            return new PdfComment(lexer.ReadUntilEol());
        }

        public override string ToString() => $"%{_text}";
    }
}
