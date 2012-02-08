using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SafeRapidPdf;
using SafeRapidPdf.Document;

namespace PdfStructureViewer.Views
{
	/// <summary>
	/// Interaction logic for DocumentStrcutureUserControl.xaml
	/// </summary>
	public partial class DocumentStructureUserControl : UserControl
	{
		public DocumentStructureUserControl()
		{
			InitializeComponent();
		}

		private PdfDocument _document;
		public PdfDocument DocumentStructure 
		{
			get { return _document; }
			set { _document = value; RefreshControl(); }
		}

		private void RefreshControl()
		{
			treeView.ItemsSource = _document.Items;
		}
	}
}
