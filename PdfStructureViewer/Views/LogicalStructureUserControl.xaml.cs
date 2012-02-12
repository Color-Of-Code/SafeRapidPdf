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
using SafeRapidPdf.Logical;

namespace PdfStructureViewer.Views
{
	/// <summary>
	/// Interaction logic for LogicalStructureUserControl.xaml
	/// </summary>
	public partial class LogicalStructureUserControl : UserControl
	{
		public LogicalStructureUserControl()
		{
			InitializeComponent ();
		}

		private PdfStructure _structure;
		public PdfStructure LogicalStructure 
		{
			get { return _structure; }
			set { _structure = value; RefreshControl(); }
		}

		private void RefreshControl()
		{
			treeView.ItemsSource = _structure.Items;
		}

		private void treeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}


	}
}
