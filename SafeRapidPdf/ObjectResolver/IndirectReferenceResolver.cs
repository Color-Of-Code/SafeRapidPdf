using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SafeRapidPdf.Primitives;

namespace SafeRapidPdf.ObjectResolver
{
	internal class IndirectReferenceResolver : IIndirectReferenceResolver
	{
		public IndirectReferenceResolver(Lexical.ILexer lexer)
		{
			_indirectObjects = new Dictionary<String, PdfIndirectObject>();
			_lexer = lexer;

			RetrieveXRef();
		}

		private IDictionary<String, PdfIndirectObject> _indirectObjects;
		private Lexical.ILexer _lexer;
		private PdfXRef _xref;

		public void InsertObject(PdfIndirectObject obj)
		{
			if (obj == null)
				throw new Exception("Parser error: this object must be an indirect object");
			String key = PdfXRef.BuildKey(obj.ObjectNumber, obj.GenerationNumber);
			_indirectObjects[key] = obj;
		}

		public PdfIndirectObject GetObject(int objectNumber, int generationNumber)
		{
			PdfIndirectObject obj = FindObject(objectNumber, generationNumber);
			if (obj == null)
			{
				// entry from XRef
				_lexer.PushPosition(_xref.GetOffset(objectNumber, generationNumber));
				// load the object if it was not yet found
				obj = PdfIndirectObject.Parse(_lexer);
				_lexer.PopPosition();
			}
			return obj;
		}

		private PdfIndirectObject FindObject(int objectNumber, int generationNumber)
		{
			String key = PdfXRef.BuildKey(objectNumber, generationNumber);
			PdfIndirectObject obj;
			if (_indirectObjects.TryGetValue(key, out obj))
				return obj;
			return null;
		}

		private void RetrieveXRef()
		{
			StartXRef = RetrieveStartXRef();

			_lexer.PushPosition(StartXRef);
			_xref = PdfXRef.Parse(_lexer);
			_lexer.PopPosition();
		}

		private long RetrieveStartXRef()
		{
			_lexer.PushPosition(-100);
			// determine StartXRef
			long result = -1;
			string t = null;
			do
			{
				t = _lexer.ReadToken();
			}
			while (t != null && t != "startxref");
			if (t == "startxref")
				result = long.Parse(_lexer.ReadToken());
			_lexer.PopPosition();
			return result;
		}


		private long StartXRef;
	}
}
