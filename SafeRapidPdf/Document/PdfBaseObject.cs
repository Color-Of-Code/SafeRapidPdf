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

		public PdfObjectType ObjectType
		{
			get; private set;
		}

		public bool IsContainer { get; protected set; }

		public String Text
		{
			get
			{
				return ToString();
			}
		}

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
