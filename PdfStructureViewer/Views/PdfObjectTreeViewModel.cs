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
	public class PdfObjectTreeViewModel
	{
		#region Data

		readonly ReadOnlyCollection<PdfObjectViewModel> _firstGeneration;
		readonly PdfObjectViewModel _root;

		string _searchText = String.Empty;

		#endregion // Data

		#region Constructor

		public PdfObjectTreeViewModel(IPdfObject rootPerson)
		{
			_root = new PdfObjectViewModel(rootPerson);

			_firstGeneration = new ReadOnlyCollection<PdfObjectViewModel>(
				new PdfObjectViewModel[] 
                { 
                    _root 
                });
		}

		#endregion

		#region Properties

		#region FirstGeneration

		public ReadOnlyCollection<PdfObjectViewModel> FirstGeneration
		{
			get
			{
				return _firstGeneration;
			}
		}

		#endregion

		#endregion
	}
}
