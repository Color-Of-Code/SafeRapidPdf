using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

using SafeRapidPdf;
using SafeRapidPdf.File;

namespace PdfStructureViewer.Views
{
	public class PdfObjectViewModel: INotifyPropertyChanged
	{
		#region Constructors

		public PdfObjectViewModel(IPdfObject pdfObject)
			: this(pdfObject, null)
		{
		}

		private PdfObjectViewModel(IPdfObject pdfObject, PdfObjectViewModel parent)
		{
			_pdfObject = pdfObject;
			_parent = parent;

			if (_pdfObject.Items != null)
				_items = new ReadOnlyCollection<PdfObjectViewModel>(
					(from child in _pdfObject.Items
					 select new PdfObjectViewModel(child, this))
					 .ToList<PdfObjectViewModel>());
			else
				_items = null;
		}

		#endregion // Constructors

		#region PdfObject Properties

		public ReadOnlyCollection<PdfObjectViewModel> Items
		{
			get
			{
				return _items;
			}
		}

		public String Text
		{
			get
			{
				return _pdfObject.Text;
			}
		}

		public PdfObjectType ObjectType
		{
			get
			{
				return _pdfObject.ObjectType;
			}
		}

		public IPdfObject Object
		{
			get
			{
				return _pdfObject;
			}
		}

		#endregion

		#region Presentation Members

		#region IsExpanded

		public bool IsExpanded
		{
			get
			{
				return _isExpanded;
			}
			set
			{
				if (value != _isExpanded)
				{
					_isExpanded = value;
					this.OnPropertyChanged("IsExpanded");
				}

				if (_isExpanded && _parent != null)
					_parent.IsExpanded = true;
			}
		}

		#endregion

		#region IsSelected

		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				if (value != _isSelected)
				{
					_isSelected = value;
					this.OnPropertyChanged("IsSelected");
				}
			}
		}

		#endregion

		#region Parent

		public PdfObjectViewModel Parent
		{
			get
			{
				return _parent;
			}
		}

		#endregion

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region Data

		private readonly ReadOnlyCollection<PdfObjectViewModel> _items;
		private readonly PdfObjectViewModel _parent;
		private readonly IPdfObject _pdfObject;

		private bool _isExpanded;
		private bool _isSelected;

		#endregion // Data
	}
}
