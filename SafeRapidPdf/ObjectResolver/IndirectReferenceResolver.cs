using SafeRapidPdf.File;

namespace SafeRapidPdf.ObjectResolver
{
    internal class IndirectReferenceResolver : IIndirectReferenceResolver
	{
		public IndirectReferenceResolver(Lexical.ILexer lexer)
		{
			_lexer = lexer;

            // in the case of linearized PDFs there are additional linearized structures added
            // to the PDF.
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
            var token = _lexer.ReadToken();
            if (token == "xref")
            {
                // we have an uncompressed xref table
                _xref = PdfXRef.Parse(_lexer);
            }
            else
            {
                // the xref is inside a compressed stream...
                _lexer.PopPosition();
                _lexer.PushPosition(StartXRef);
                // decode the object
                var xrefStream = PdfObject.ParseAny(_lexer);
                _xref = PdfXRef.Parse(xrefStream);
            }
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