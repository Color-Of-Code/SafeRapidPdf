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
			_dictionary = new Dictionary<string, PdfObject>();
			String token;
			while ((token = lexer.PeekToken()) != ">>")
			{
				PdfName name = new PdfName(lexer);
				PdfObject value = PdfObject.Parse(lexer);
				_dictionary.Add(name.Text, value);
			}
			lexer.Expects(">>");
        }

		public ReadOnlyCollection<string> Keys { get { return _dictionary.Keys.ToList().AsReadOnly(); } }

		public PdfObject this[string name]
		{
			get
			{
				return _dictionary[name];
			}
		}

        private IDictionary<string, PdfObject> _dictionary;
    }
}
