using System;
using System.Collections.ObjectModel;

namespace SafeRapidPdf.Document
{
    public abstract class PdfBaseObject : IPdfObject
	{
		protected PdfBaseObject(PdfObjectType type)
		{
			ObjectType = type;
		}

		public PdfObjectType ObjectType { get; }

        public bool IsContainer { get; protected set; }

        public string Text => ToString();

        public virtual ReadOnlyCollection<IPdfObject> Items
		{
			get 
			{
				if (!IsContainer)
					return null;
				throw new NotImplementedException();
			}
		}
	}
}
