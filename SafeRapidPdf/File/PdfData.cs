namespace SafeRapidPdf.File
{
    public class PdfData : PdfObject
	{
		private PdfData(byte[] data)
			: base(PdfObjectType.Data)
		{
			Data = data;
		}

		public byte[] Data
		{
			get;
			private set;
		}

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
