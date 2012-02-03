using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfStartXRef : PdfObject
	{
		public PdfStartXRef(Lexical.ILexer lexer)
		{
			IsContainer = true;
			lexer.Expects("startxref");
			Numeric = new PdfNumeric(lexer);
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
