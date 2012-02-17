using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SafeRapidPdf.File;

namespace SafeRapidPdf.ObjectResolver
{
	internal class IndirectReferenceResolver : IIndirectReferenceResolver
	{
		public IndirectReferenceResolver(Lexical.ILexer lexer)
		{
			_lexer = lexer;

			RetrieveXRef();
		}

		private Lexical.ILexer _lexer;
		private PdfXRef _xref;

		public PdfIndirectObject GetObject(int objectNumber, int generationNumber)
		{
			// entry from XRef
			_lexer.PushPosition(_xref.GetOffset(objectNumber, generationNumber));
			// load the object if it was not yet found
			var obj = PdfIndirectObject.Parse(_lexer);
			_lexer.PopPosition();
			return obj;
		}

		private void RetrieveXRef()
		{
			StartXRef = RetrieveStartXRef();

			_lexer.PushPosition(StartXRef);
			_lexer.Expects("xref");
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
