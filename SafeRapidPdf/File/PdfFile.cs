using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using SIO = System.IO;

namespace SafeRapidPdf.File
{
    /// <summary>
    /// Represents the physical structure of a PDF. Contains the objects present
    /// in the file and allows direct retrieval of indirect references.
    /// The file itself is considered as a PDF object.
    /// </summary>
    public class PdfFile : IPdfObject, IIndirectReferenceResolver
	{
		private PdfFile(ReadOnlyCollection<IPdfObject> objects)
		{
			Items = objects;

			// build up the fast object lookup dictionary
			_indirectObjects = new Dictionary<String, PdfIndirectObject>();
			foreach (var obj in Items.OfType<PdfIndirectObject>())
				InsertObject(obj);
			SetResolver(this);
		}

		private void SetResolver(IPdfObject obj)
		{
			if (obj.IsContainer)
			{
				foreach (IPdfObject item in obj.Items)
				{
                    if (item is PdfIndirectReference iref)
                    {
                        iref.Resolver = this;
                    }
                    else
                    {
                        SetResolver(item);
                    }
                }
			}
		}

		public static PdfFile Parse(SIO.Stream reader, EventHandler<ProgressChangedEventArgs> progress = null)
		{
			progress?.Invoke(null, new ProgressChangedEventArgs(0, null));

			Stopwatch watch = new Stopwatch();
			watch.Start();
			var lexer = new Lexical.LexicalParser (reader);

			List<IPdfObject> objects = new List<IPdfObject>();

			// check that this stuff is really looking like a PDF
			lexer.Expects("%");
			PdfComment comment = PdfComment.Parse(lexer);
			if (!comment.Text.StartsWith("%PDF-"))
				throw new Exception("PDF header missing");
			objects.Add(comment);

			bool lastObjectWasOEF = false;
			while (true)
			{
				var obj = PdfObject.ParseAny(lexer);

				if (obj == null)
				{
					if (lastObjectWasOEF)
						break;
					else
						throw new Exception("End of file reached without EOF marker");
				}

				objects.Add(obj);

				progress?.Invoke(null, new ProgressChangedEventArgs(lexer.Percentage, null));

				lastObjectWasOEF = false;
				if (obj is PdfComment cmt)
				{
					if (cmt.IsEOF)
					{
						// a linearized or updated document might contain several EOF markers
						lastObjectWasOEF = true;
					}
				}
			}
			progress?.Invoke(null, new ProgressChangedEventArgs(100, null));
			watch.Stop();

			PdfFile file = new PdfFile(objects.AsReadOnly())
			{
				ParsingTime = watch.Elapsed.TotalSeconds
			};

			return file;
		}

		public static PdfFile Parse(String pdfFilePath, EventHandler<ProgressChangedEventArgs> progress = null)
		{
			using (SIO.Stream reader = SIO.File.Open(pdfFilePath, SIO.FileMode.Open, SIO.FileAccess.Read, SIO.FileShare.Read))
			{
				return Parse(reader, progress);
			}
		}

		/// <summary>
		/// Parsing time in seconds
		/// </summary>
		public Double ParsingTime { get; private set; }

        public String Version => Items.First().ToString();

        public ReadOnlyCollection<IPdfObject> Items { get; private set; }

        public string Text => "File";

        public bool IsContainer => true;

        public PdfObjectType ObjectType => PdfObjectType.File;

        private IDictionary<String, PdfIndirectObject> _indirectObjects;

		private void InsertObject(PdfIndirectObject obj)
		{
			if (obj == null)
				throw new Exception("This object must be an indirect object");
			String key = PdfXRef.BuildKey(obj.ObjectNumber, obj.GenerationNumber);
			_indirectObjects[key] = obj;
		}

		public PdfIndirectObject GetObject(int objectNumber, int generationNumber)
		{
			String key = PdfXRef.BuildKey(objectNumber, generationNumber);
			return _indirectObjects[key];
		}
	}
}
