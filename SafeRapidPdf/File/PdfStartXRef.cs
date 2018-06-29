using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SafeRapidPdf.File
{
    public sealed class PdfStartXRef : PdfObject
	{
		private PdfStartXRef(PdfNumeric value)
			: base(PdfObjectType.StartXRef)
		{
			IsContainer = true;
			Numeric = value;
		}

		public static PdfStartXRef Parse(Lexical.ILexer lexer)
		{
			var n = PdfNumeric.Parse(lexer);
			return new PdfStartXRef(n);
		}

		public PdfNumeric Numeric { get; private set; }

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
                var list = new List<IPdfObject>(1)
                {
                    Numeric
                };

                return list.AsReadOnly();
			}
		}

		public override string ToString()
		{
			return "startxref";
		}
	}
}
