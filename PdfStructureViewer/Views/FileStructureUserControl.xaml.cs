using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using SafeRapidPdf.File;
using SafeRapidPdf.Document;

namespace PdfStructureViewer.Views
{
	/// <summary>
	/// Interaction logic for FileStructureUserControl.xaml
	/// </summary>
	public partial class FileStructureUserControl: UserControl
	{
		public FileStructureUserControl()
		{
			InitializeComponent();
		}

		public Image ImageControl
		{
			get;
			set;
		}

		private PdfFile _file;
		public PdfFile FileStructure
		{
			get
			{
				return _file;
			}
			set
			{
				_file = value;
				RefreshControl();
			}
		}

		private void RefreshControl()
		{
			treeView.ItemsSource = _file.Items;
		}

		private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			// if the object is a stream of image type, display it on the side
		}

		private void textBoxQuery_TextChanged(object sender, TextChangedEventArgs e)
		{
			ICollectionView view = CollectionViewSource.GetDefaultView(treeView.ItemsSource);
			if (view == null)
				return;
			String query = textBoxQuery.Text;
			if (String.IsNullOrWhiteSpace(query))
			{
				view.Filter = null;
			}
			else
			{
				query = query.Trim();
				bool inverted = false;
				if (query.StartsWith("!"))
				{
					inverted = true;
					query = query.Substring(1).Trim();
				}
				view.Filter = o =>
				{
					var pdfObject = o as IPdfObject;
					bool predicate = o.ToString().Contains(query);
					return inverted ^ predicate;
				};
			}
		}
	}
}
