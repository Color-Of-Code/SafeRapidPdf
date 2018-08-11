using System;

namespace SafeRapidPdf.File
{
    /// <summary>
    /// Comments start with % and end on EOL char (CR or LF)
    /// </summary>
    public sealed class PdfComment : PdfObject
	{
		private PdfComment(string text)
			: base(PdfObjectType.Comment)
		{
			_text = text;
		}

		public static PdfComment Parse(Lexical.ILexer lexer)
		{
			return new PdfComment(lexer.ReadUntilEol());
		}

		private readonly string _text;

        public bool IsEOF => _text == "%EOF";

        public override string ToString()
		{
            return $"%{_text}";
		}
	}
}
