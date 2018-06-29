namespace SafeRapidPdf.File
{
    public class PdfNull : PdfObject
	{
		private PdfNull()
			: base(PdfObjectType.Null)
		{
		}

		public static readonly PdfNull Null = new PdfNull();

		public static PdfNull Parse(Lexical.ILexer lexer)
		{
			lexer.Expects("null");
			return Null;
		}

		public override string ToString()
		{
			return "null";
		}
	}
}
