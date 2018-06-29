using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SafeRapidPdf.File
{
    public sealed class PdfXRefSection : PdfObject
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

		public int FirstId { get; }

		public int Size { get; }

		private List<IPdfObject> _entries;
		public override ReadOnlyCollection<IPdfObject> Items 
		{
            get => _entries.AsReadOnly();
        }

		public override string ToString()
		{
            return $"{FirstId} {Size}";
		}
	}
}