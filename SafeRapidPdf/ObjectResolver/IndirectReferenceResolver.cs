using System;
using SafeRapidPdf.File;

namespace SafeRapidPdf.ObjectResolver
{
    internal class IndirectReferenceResolver : IIndirectReferenceResolver
    {
        public IndirectReferenceResolver(Lexical.ILexer lexer)
        {
            _lexer = lexer;

            _lexer.PushPosition(0);

            TryParseLinearizationHeader();

            // in the case of linearized PDFs there are additional linearized structures added
            // to the PDF. Otherwise we take the non linearized approach
            if (_linearizationHeader != null)
                RetrieveXRefLinearized();
            else
                RetrieveXRef();

            _lexer.PopPosition();
        }

        private void TryParseLinearizationHeader()
        {
            _linearizationHeader = null;

            try
            {
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
			catch
			{
				// ignore... I know bad style
				// in this case the linearization header is assume to not have been found
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
            var firstPageXRef = PdfObject.ParseAny(_lexer);
            var mainXRefPosition = _linearizationHeader["T"] as PdfNumeric;
            _lexer.PushPosition((long)mainXRefPosition.Value);
            var mainXRef = PdfObject.ParseAny(_lexer);
            _lexer.PopPosition();
        }

        // returns true if an xref was found false otherwise
        private void RetrieveXRef()
        {
            _xref = null;

            // only necessary if not linearized
            StartXRef = RetrieveStartXRef();
            // if the xref was not found, early exit
            if (StartXRef == -1)
                return;

            _lexer.PushPosition(StartXRef);
            var token = _lexer.ReadToken();
            if (token == "xref")
            {
                // we have an uncompressed xref table
                _xref = PdfXRef.Parse(_lexer);
            }
            else
            {
                // maybe there is no xref
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