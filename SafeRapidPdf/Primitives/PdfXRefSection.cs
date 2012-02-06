using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfXRefSection : PdfObject
	{
		private PdfXRefSection(int firstId, int size, List<IPdfObject> entries)
			: base(PdfObjectType.XRefSection)
		{
			IsContainer = true;

			FirstId = firstId;
			Size = size;
			_entries = entries;
		}

		public static PdfXRefSection Parse(Lexical.ILexer lexer)
		{
			int firstId = int.Parse(lexer.ReadToken());
			int size = int.Parse(lexer.ReadToken());
			var entries = new List<IPdfObject>(size);
			for (int i = 0; i < size; i++)
			{
				PdfXRefEntry entry = PdfXRefEntry.Parse(firstId + i, lexer);
				entries.Add(entry);
			}
			return new PdfXRefSection(firstId, size, entries);
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
