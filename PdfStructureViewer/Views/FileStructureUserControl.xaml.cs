using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
			treeView.ItemsSource = new PdfObjectTreeViewModel(_file).FirstGeneration;
			ApplyFilter();
		}

		private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			// if the object is a stream of image type, display it on the side
			PdfObjectViewModel objectViewModel = e.NewValue as PdfObjectViewModel;
			if (objectViewModel == null)
				return;
			PdfStream stream = objectViewModel.Object as PdfStream;
			if (stream != null)
			{
				PdfDictionary dic = stream.StreamDictionary;
				var filter = dic["Filter"];
				if (filter.Text.Contains("DCTDecode"))
				{
					var jpegData = new MemoryStream(stream.Data.Data);
					var decoder = new JpegBitmapDecoder(jpegData, BitmapCreateOptions.None, BitmapCacheOption.Default);
					var imageSource = decoder.Frames[0];
					imageSource.Freeze();
					ImageControl.Source = imageSource;
				}
			}
		}

		private void textBoxQuery_TextChanged(object sender, TextChangedEventArgs e)
		{
			ApplyFilter();
		}

		private void ApplyFilter()
		{
			object source = treeView.ItemsSource;
			String query = textBoxQuery.Text;
			query = query.Trim();
			bool inverted = false;
			if (query.StartsWith("!"))
			{
				inverted = true;
				query = query.Substring(1).Trim();
			}
			ApplyFilterToItems(source, query, inverted);
		}

		private static bool ApplyFilterToItems(object source, String query, bool inverted)
		{
			if (source == null)
				return false;
			ICollectionView view = CollectionViewSource.GetDefaultView(source);
			if (view == null)
				return true;
			bool show = false;
			if (String.IsNullOrWhiteSpace(query))
			{
				view.Filter = o =>
				{
					var pdfObjectVM = o as PdfObjectViewModel;
					ApplyFilterToItems(pdfObjectVM.Items, query, inverted);
					pdfObjectVM.IsExpanded = false;
					return true;
				};
				view.Filter = null;
				show = true;
			}
			else
			{
				view.Filter = o =>
				{
					var pdfObjectVM = o as PdfObjectViewModel;
					bool predicate = pdfObjectVM.Object.ToString().Contains(query);
					bool result = inverted ^ predicate;
					if (!result)
						result = ApplyFilterToItems(pdfObjectVM.Items, query, inverted);
					else
						ApplyFilterToItems(pdfObjectVM.Items, String.Empty, inverted);
					pdfObjectVM.IsExpanded = result;
					show = show || result;
					return result;
				};
			}
			return show;
		}

		private void treeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var nodeVM = treeView.SelectedItem as PdfObjectViewModel;
			if (nodeVM != null)
			{
				var node = nodeVM.Object as PdfIndirectReference;
				if (node != null)
				{
					//var obj = node.ReferencedObject;
					//var view = CollectionViewSource.GetDefaultView(treeView.ItemsSource);
					//node.IsExpanded = true;
					//node.IsSelected = true;
					//bool isInView = view.MoveCurrentTo(obj);
					//view.Refresh();
				}
			}
		}
	}
}
