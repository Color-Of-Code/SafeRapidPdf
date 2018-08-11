namespace SafeRapidPdf.File
{
    public sealed class PdfData : PdfObject
    {
        private PdfData(byte[] data)
            : base(PdfObjectType.Data)
        {
            Data = data;
        }

        public byte[] Data { get; }

        public static PdfData Parse(Lexical.ILexer lexer, int length)
        {
            byte[] data = lexer.ReadBytes(length);
            return new PdfData(data);
        }

        public override string ToString()
        {
            return "Raw data";
        }
    }
}
