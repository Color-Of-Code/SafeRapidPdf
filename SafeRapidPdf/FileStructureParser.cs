using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace SafeRapidPdf
{
	using SafeRapidPdf.Primitives;

	public class FileStructureParser : IFileStructureParser, IIndirectReferenceResolver
	{
		public FileStructureParser(String path)
		{
			_path = path;
		}

		private Lexical.ILexer _lex;
		private Stream _reader;
		private String _path;
		private PdfXRef _xref;

		public Pdf.FileStructure Parse()
		{
			using (_reader = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				_lex = new Lexical.LexicalParser(_reader, this);

				List<PdfObject> objects = new List<PdfObject>();
				_indirectObjects = new Dictionary<String, PdfIndirectObject>();

				// check that this stuff is really looking like a PDF
				PdfComment comment = PdfObject.Parse(_lex) as PdfComment;
				if (comment == null || !comment.Text.StartsWith("%PDF-"))
					throw new Exception("PDF header missing");
				objects.Add(comment);

				RetrieveXRef();

				bool lastObjectWasOEF = false;
				while (true)
				{
					var obj = PdfObject.Parse(_lex);

					if (obj == null)
					{
						if (lastObjectWasOEF)
							break;
						else
							throw new Exception("End of file reached without EOF marker");
					}
					
					if (obj is PdfIndirectObject)
					{
						InsertObject(obj as PdfIndirectObject);
					}

					objects.Add(obj);

					lastObjectWasOEF = false;
					if (obj is PdfComment)
					{
						PdfComment cmt = obj as PdfComment;
						if (cmt.IsEOF)
						{
							// a linearized or updated document might contain several EOF markers
							lastObjectWasOEF = true;
						}
					}
				}
				return new Pdf.FileStructure(objects.AsReadOnly()); ;
			}
		}

		private void RetrieveXRef()
		{
			long position = Position;

			StartXRef = RetrieveStartXRef();
			// read XRef
			Position = StartXRef;
			_xref = PdfObject.Parse(_lex) as PdfXRef;

			Position = position;
		}

		private long RetrieveStartXRef()
		{
			// determine StartXRef
			_reader.Seek(-100, SeekOrigin.End);
			long result = -1;
			string t = null;
			do
			{
				t = _lex.ReadToken();
			}
			while (t != null && t != "startxref");
			if (t == "startxref")
				result = long.Parse(_lex.ReadToken());
			return result;
		}


		private long StartXRef;

		private IDictionary<String,PdfIndirectObject> _indirectObjects;

		private void InsertObject(PdfIndirectObject obj)
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
				long lastPosition = Position;
				// load the object if it was not yet found
				Position = _xref.GetOffset(objectNumber, generationNumber); // entry from XRef
				obj = new PdfIndirectObject(_lex);
				Position = lastPosition;
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

		private long Position
		{
			get
			{
				return _reader.Position;
			}
			set
			{
				_reader.Seek(value, SeekOrigin.Begin);
			}
		}
	}
}
