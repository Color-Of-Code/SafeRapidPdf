using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.File
{
	public class PdfTrailer : PdfObject
	{
		private PdfTrailer(PdfDictionary dictionary)
			: base(PdfObjectType.Trailer)
		{
			IsContainer = true;
			Content = dictionary;
		}

		public static PdfTrailer Parse(Lexical.ILexer lexer)
		{
			lexer.Expects("trailer");
			var dictionary = PdfDictionary.Parse(lexer);
			return new PdfTrailer(dictionary);
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
