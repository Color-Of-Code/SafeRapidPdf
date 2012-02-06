using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfStartXRef : PdfObject
	{
		private PdfStartXRef(PdfNumeric value)
			: base(PdfObjectType.StartXRef)
		{
			IsContainer = true;
			Numeric = value;
		}

		public static PdfStartXRef Parse(Lexical.ILexer lexer)
		{
			lexer.Expects("startxref");
			var n = PdfNumeric.Parse(lexer);
			return new PdfStartXRef(n);
		}

		public PdfNumeric Numeric { get; private set; }

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				var list = new List<IPdfObject>();
				list.Add(Numeric);
				return list.AsReadOnly();
			}
		}

		public override string ToString()
		{
			return "startxref";
		}
	}
}
