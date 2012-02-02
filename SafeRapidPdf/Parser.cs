using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace SafeRapidPdf
{
	using SafeRapidPdf.Primitives;

	public class Parser : IPdfParser
	{
		public Parser(String path)
		{
			_path = path;
		}

		private Stream _reader;
		private String _path;
		private PdfXRef _xref;

		public Pdf.Document Parse()
		{
			using (_reader = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				List<PdfObject> objects = new List<PdfObject>();
				_indirectObjects = new Dictionary<String, PdfIndirectObject>();

				// check that this stuff is really looking like a PDF
				PdfComment comment = ReadPdfObject() as PdfComment;
				if (comment == null || !comment.Text.StartsWith("PDF-"))
					throw new Exception("PDF header missing");
				objects.Add(comment);

				RetrieveXRef();

				bool lastObjectWasOEF = false;
				while (true)
				{
					var obj = ReadPdfObject();

					if (obj == null)
					{
						if (lastObjectWasOEF)
							break;
						else
							throw new Exception("End of file reached without EOF marker");
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
				return new Pdf.Document(comment.Text, objects.AsReadOnly()); ;
			}
		}

		private void RetrieveXRef()
		{
			long position = Position;

			StartXRef = RetrieveStartXRef();
			// read XRef
			Position = StartXRef;
			_xref = ReadPdfObject() as PdfXRef;

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
				t = ReadToken();
			}
			while (t != null && t != "startxref");
			if (t == "startxref")
				result = long.Parse(ReadToken());
			_tokens.Clear();
			return result;
		}

		public PdfObject ReadPdfObject()
		{
			String token = ReadToken();
			return ReadPdfObject(token);
		}

		public PdfObject ReadPdfObject(String token)
		{
			if (token == null) return null;

			Primitives.PdfObject obj = null;
			switch (token)
			{
				// null object
				case "null":
					obj = PdfObject.Null;
					break;

				// commment
				case "%":
					obj = new PdfComment(_reader);
					break;

				// boolean
				case "true":
				case "false":
					obj = new PdfBoolean(token == "true");
					break;

				// indirect object
				case "obj":
					{
						if (_tokens.Count != 2)
							throw new Exception("Expected Object and Generation number");
						int genN = int.Parse(_tokens.Pop());
						int objN = int.Parse(_tokens.Pop());
						obj = new PdfIndirectObject(objN, genN, this);
						InsertObject(obj as PdfIndirectObject);
						break;
					}
				// indirect reference
				case "R":
					{
						if (_tokens.Count != 2)
							throw new Exception("Expected Object and Generation number");
						int genN = int.Parse(_tokens.Pop());
						int objN = int.Parse(_tokens.Pop());
						obj = new PdfIndirectReference(objN, genN);
						break;
					}

				// dictionary
				case "<<":
					obj = new PdfDictionary(this);
					break;

				// array
				case "[":
					obj = new PdfArray(this);
					break;

				// string
				case "<":
					obj = new PdfHexadecimalString(this);
					break;
				case "(":
					obj = new PdfLiteralString(this);
					break;
				case "/":
					obj = new PdfName(this);
					break;

				case "stream":
					obj = new PdfStream(_lastObject as PdfDictionary, this);
					break;

				case "xref":
					obj = new PdfXRef(this);
					break;
				case "trailer":
					obj = new PdfTrailer(this);
					break;
				case "startxref":
					long result = long.Parse(ReadToken());
					if (StartXRef != result && result != 0) // 0 is used for linearized pdfs
						throw new Exception("Parser error: startxref already found with another value");
					obj = PdfObject.Null;
					break;

				case ")":
				case ">":
				case ">>":
				case "]":
				case "}":
				case "endstream":
				case "endobj":
					throw new Exception("Parser error: out of sync");

				default:
					// must be an integer or double value
					obj = new PdfNumeric(token);
					break;
			}

			if (_tokens.Count != 0)
				throw new Exception("Parsing error, token stack not empty");
			if (obj == null)
				throw new Exception("Parsing error, could not read object");
			_lastObject = obj;
			return obj;
		}

		private long StartXRef;

		private PdfObject _lastObject = null;
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
				obj = ReadPdfObject() as PdfIndirectObject;
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

		public String ReadToken()
		{
			SkipWhitespaces();

			int b = _reader.ReadByte();
			if (b == -1)
				return null;

			char c = (char)b;
			switch (c)
			{
				case '%': return "%";
				case '/': return "/";
				case '[': return "[";
				case ']': return "]";
				case '(': return "(";
				case ')': return ")";
				case '<':
					b = _reader.ReadByte();
					if ((char)b == '<')
						return "<<";
					Putc();
					return "<";
				case '>':
					b = _reader.ReadByte();
					if ((char)b == '>')
						return ">>";
					Putc();
					return ">";
			}

			Putc();
			string token = ParseToken();
			if (IsInteger(token))
			{
				long pos = _reader.Position;
				string token2 = ParseToken();
				if (IsInteger(token2))
				{
					// should be "obj" or "R"
					string token3 = ParseToken();
					if (token3 == "obj" || token3 == "R")
					{
						_tokens.Push(token);
						_tokens.Push(token2);
						token = token3;
					}
					else
					{
						// rewind
						_reader.Seek(pos, SeekOrigin.Begin);
					}
				}
				else
				{
					// rewind
					_reader.Seek(pos, SeekOrigin.Begin);
				}
			}

			if (token == String.Empty)
			{
				if (_reader.ReadByte() == -1) // end of file
					return null;
				throw new Exception("Token may not be empty");
			}

			// position to start of stream data
			if (token == "stream")
			{
				while (Chars.Test.IsEol(_reader.ReadByte())) ;
			}

			// one step back because one char has been read too far
			Putc();

			return token;
		}

		private void Putc()
		{
			_reader.Seek(-1, SeekOrigin.Current);
		}

		private static bool IsInteger(string token)
		{
			int result;
			if (int.TryParse(token, out result))
				return true;
			return false;
		}

		private String ParseToken()
		{
			SkipWhitespaces();
			int b = _reader.ReadByte();
			StringBuilder token = new StringBuilder();
			if (Chars.Test.IsDelimiter(b))
			{
				token.Append((char)b);
				b = _reader.ReadByte();
			}
			else
			{
				while (Chars.Test.IsRegular(b))
				{
					token.Append((char)b);
					b = _reader.ReadByte();
				}
			}
			return token.ToString();
		}

		private void SkipWhitespaces()
		{
			// skip whitespaces
			int c = 0;
			do
			{
				c = _reader.ReadByte();
			} while (Chars.Test.IsWhitespace(c));
			if (c != -1)
				Putc();
		}

		private Stack<string> _tokens = new Stack<string>();

		public byte[] ReadBytes(int length)
		{
			byte[] buffer = new byte[length];
			if (_reader.Read(buffer, 0, length) != length)
				throw new Exception("Parser error: could not read the full amount of bytes");
			return buffer;
		}

		public char ReadChar()
		{
			return (char)_reader.ReadByte();
		}


		public long Position
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
