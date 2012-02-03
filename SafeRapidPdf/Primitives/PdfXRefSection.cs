using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfXRefSection : PdfObject
	{
		public PdfXRefSection(Lexical.ILexer lexer)
		{
			IsContainer = true;

			FirstId = int.Parse(lexer.ReadToken());
			Size = int.Parse(lexer.ReadToken());
			_entries = new List<IPdfObject>(Size);
			for (int i = 0; i < Size; i++)
			{
				PdfXRefEntry entry = new PdfXRefEntry(FirstId + i, lexer);
				_entries.Add(entry);
			}
		}

		public int FirstId { get; private set; }
		public int Size { get; private set; }

		private List<IPdfObject> _entries;
		public override ReadOnlyCollection<IPdfObject> Items 
		{
			get
			{
				return _entries.AsReadOnly(); 
			}
		}

		public override string ToString()
		{
			return String.Format("{0} {1}", FirstId, Size);
		}
	}
}
