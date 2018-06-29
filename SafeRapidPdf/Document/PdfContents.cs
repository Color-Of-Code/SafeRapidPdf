using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using SafeRapidPdf.File;

namespace SafeRapidPdf.Document
{
    public class PdfContents : PdfBaseObject
	{
		public PdfContents(IPdfObject obj)
			: base(PdfObjectType.Contents)
		{
			IsContainer = true;
			if (obj is PdfIndirectReference)
			{
				obj = (obj as PdfIndirectReference).ReferencedObject.Object;
			}
            
            if (obj is PdfArray array)
            {
                Streams = array.Items;
            }
            else
            {
                if (obj is PdfStream stream)
                {
                    var list = new List<IPdfObject>(1)
                    {
                        stream
                    };

                    Streams = list.AsReadOnly();
                }
                else
                {
                    throw new Exception("Contents must be either a stream or an array of streams");
                }
            }
        }

		public ReadOnlyCollection<IPdfObject> Streams { get; }

        public override ReadOnlyCollection<IPdfObject> Items => Streams;

		public override string ToString()
		{
			return "Contents";
		}
	}
}
