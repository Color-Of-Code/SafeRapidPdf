using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfXRef : PdfObject
	{
		public PdfXRef(Lexical.ILexer lexer)
		{
			IsContainer = true;

			lexer.Expects("xref");

			_sections = new List<PdfXRefSection>();
			String token = lexer.PeekToken();
			while (Char.IsDigit(token[0]))
			{
				_sections.Add(PdfXRefSection.Parse(lexer));
				token = lexer.PeekToken();
			}

			// create the access table
			foreach (var section in _sections)
			{
				foreach (var entryItem in section.Items)
				{
					PdfXRefEntry entry = entryItem as PdfXRefEntry;
					if (entry.InUse)
					{
						String key = BuildKey(entry.ObjectNumber, entry.GenerationNumber);
						_offsets.Add(key, entry.Offset);
					}
				}
			}
		}

		private List<PdfXRefSection> _sections;

		public long GetOffset(int objectNumber, int generationNumber)
		{
			String key = BuildKey(objectNumber, generationNumber);
			return _offsets[key];
		}

		public static String BuildKey(int objectNumber, int generationNumber)
		{
			return String.Format("{0:0000000000}_{1:00000}", objectNumber, generationNumber);
		}

		private Dictionary<String, long> _offsets = new Dictionary<string, long>();

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				return _sections.ConvertAll(x => x as IPdfObject).AsReadOnly();
			}
		}

		public override string ToString()
		{
			return "xref";
		}
	}
}
