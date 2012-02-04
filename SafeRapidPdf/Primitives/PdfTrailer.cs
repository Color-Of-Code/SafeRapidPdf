using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfTrailer : PdfObject
	{
		public PdfTrailer(Lexical.ILexer lexer)
		{
			IsContainer = true;
			lexer.Expects("trailer");
			Content = PdfDictionary.Parse(lexer);
		}

		public PdfDictionary Content { get; private set; }

		public override string ToString()
		{
			return "trailer";
		}

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				var list = new List<IPdfObject>();
				list.Add(Content);
				return list.AsReadOnly();
			}
		}
	}
}
