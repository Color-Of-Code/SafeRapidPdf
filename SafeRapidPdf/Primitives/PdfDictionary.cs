using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	/// <summary>
	/// Immutable type
	/// </summary>
	public class PdfDictionary : PdfObject
    {
        public PdfDictionary(Lexical.ILexer lexer)
        {
			IsContainer = true;
			lexer.Expects("<<");
			_dictionary = new List<PdfKeyValuePair>();
			String token;
			while ((token = lexer.PeekToken()) != ">>")
			{
				PdfName name = new PdfName(lexer);
				PdfObject value = PdfObject.Parse(lexer);

				_dictionary.Add(new PdfKeyValuePair(name, value));
			}
			lexer.Expects(">>");
        }

		public IPdfObject this[string name]
		{
			get
			{
				return _dictionary.First(x => x.Key.Text == name).Value;
			}
		}

		private List<PdfKeyValuePair> _dictionary;

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				return _dictionary.ConvertAll(x => x as IPdfObject).AsReadOnly();
			}
		}

		public override string ToString()
		{
			return "<<...>>";
		}
	}
}
