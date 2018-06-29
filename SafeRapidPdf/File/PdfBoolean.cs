using System;

namespace SafeRapidPdf.File
{
    public sealed class PdfBoolean : PdfObject
	{
		private PdfBoolean(Boolean value)
			: base(PdfObjectType.Boolean)
		{
			Value = value;
		}

		public static PdfBoolean Parse(String token)
		{
			if (token != "true" && token != "false")
				throw new Exception("Parser error: invalid boolean value");
			return new PdfBoolean(token == "true");
		}

		public bool Value { get; }

		public override string ToString()
		{
            return Value ? "true" : "false";
		}
	}
}
