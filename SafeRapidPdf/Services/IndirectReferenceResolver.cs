using System;
using System.Globalization;
using SafeRapidPdf.Objects;
using SafeRapidPdf.Parsing;

namespace SafeRapidPdf.Services
{
    internal class IndirectReferenceResolver : IIndirectReferenceResolver
    {
        private readonly Lexer _lexer;
        private PdfXRef _xref;
        private PdfDictionary _linearizationHeader;
        private long startXRef;

        public IndirectReferenceResolver(Lexer lexer)
        {
            _lexer = lexer;

            _lexer.PushPosition(0);

            TryParseLinearizationHeader();

            // in the case of linearized PDFs there are additional linearized structures added
            // to the PDF. Otherwise we take the non linearized approach
            if (_linearizationHeader != null)
            {
                RetrieveXRefLinearized();
            }
            else
            {
                RetrieveXRef();
            }

            _lexer.PopPosition();
        }

        public PdfXRef XRef => _xref;

        private void TryParseLinearizationHeader()
        {
            _linearizationHeader = null;

            try
            {
                // fetch the first object we see and try to parse it
                var o = PdfObject.ParseAny(_lexer);

                while (o.ObjectType == PdfObjectType.Comment)
                {
                    o = PdfObject.ParseAny(_lexer);
                }

                if (o.ObjectType == PdfObjectType.IndirectObject)
                {
                    var d = (o as PdfIndirectObject).Object;
                    if (d.ObjectType == PdfObjectType.Dictionary)
                    {
                        var dict = d as PdfDictionary;
                        var linearizedVersion = dict["Linearized"].Text;
                        if (!string.IsNullOrWhiteSpace(linearizedVersion))
                        {
                            _linearizationHeader = dict;
                        }
                    }
                }
            }
            catch
            {
                // ignore... I know bad style
                // in this case the linearization header is assume to not have been found
            }
        }

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
            var firstPageXRef = PdfObject.ParseAny(_lexer) as PdfIndirectObject;
            var mainXRefPosition = _linearizationHeader["T"] as PdfNumeric;

            _lexer.PushPosition(mainXRefPosition.ToInt64());

            var mainXRef = PdfObject.ParseAny(_lexer) as PdfIndirectObject;

            _lexer.PopPosition();

            _xref = PdfXRef.Parse((PdfStream)firstPageXRef.Object as PdfStream, (PdfStream)mainXRef.Object);
        }

        // returns true if an xref was found false otherwise
        private void RetrieveXRef()
        {
            _xref = null;

            // only necessary if not linearized
            startXRef = RetrieveStartXRef();

            // if the xref was not found, early exit
            if (startXRef == -1)
                return;

            _lexer.PushPosition(startXRef);

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
            position = Math.Max(position, -_lexer.Size); // avoid underflow
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
            {
                result = long.Parse(_lexer.ReadToken(), CultureInfo.InvariantCulture);
            }

            _lexer.PopPosition();
            return result;
        }
    }
}