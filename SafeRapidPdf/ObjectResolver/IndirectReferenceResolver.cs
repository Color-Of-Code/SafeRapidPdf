using System;
using SafeRapidPdf.File;

namespace SafeRapidPdf.ObjectResolver
{
    internal class IndirectReferenceResolver : IIndirectReferenceResolver
	{
		public IndirectReferenceResolver(Lexical.ILexer lexer)
		{
			_lexer = lexer;

			TryParseLinearizationHeader();

            // in the case of linearized PDFs there are additional linearized structures added
            // to the PDF. Otherwise we take the non linearized approach
			if (_linearizationHeader != null)
				RetrieveXRefLinearized();
			else
				RetrieveXRef();
		}

        private void TryParseLinearizationHeader()
        {
			_linearizationHeader = null;

			// we fetch the first object we see and try to parse it
			var o = PdfObject.ParseAny(_lexer);
			while (o.ObjectType == PdfObjectType.Comment)
				o = PdfObject.ParseAny(_lexer);
			if (o.ObjectType == PdfObjectType.IndirectObject)
			{
				var d = (o as PdfIndirectObject).Object;
				if (d.ObjectType == PdfObjectType.Dictionary)
				{
					var dict = d as PdfDictionary;
					var linearizedVersion = dict["Linearized"].Text;
					if (!String.IsNullOrWhiteSpace(linearizedVersion))
						_linearizationHeader = dict;
				}
			}
        }

        private Lexical.ILexer _lexer;
		private PdfXRef _xref;
		private PdfDictionary _linearizationHeader;

		public PdfIndirectObject GetObject(int objectNumber, int generationNumber)
		{
			// entry from XRef
			_lexer.PushPosition(_xref.GetOffset(objectNumber, generationNumber));
			// load the object if it was not yet found
			var obj = PdfIndirectObject.Parse(_lexer);
			_lexer.PopPosition();
			return obj;
		}

		private void RetrieveXRefLinearized()
		{
			// if we get here we can read the next object as the first xref
			// use the linearized header to jump to the main table /T offset
			// parse the xref there too
		}

		// returns true if an xref was found false otherwise
		private void RetrieveXRef()
		{
			_xref = null;

			// only necessary if not linearized
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
				throw new NotImplementedException();
				/*
				TODO: double check that there is a compressed stream with the xref
                // the xref is inside a compressed stream...
                _lexer.PopPosition();
                _lexer.PushPosition(StartXRef);
                // decode the object
                var xrefStream = PdfObject.ParseAny(_lexer);
                _xref = PdfXRef.Parse(xrefStream);
				*/
            }
            _lexer.PopPosition();
        }

		private long RetrieveStartXRef()
		{
			long position = -100; // look from end, might go wrong for very small documents
			position = System.Math.Max(position, -_lexer.Size); // avoid underflow
			_lexer.PushPosition(position);
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