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

namespace PdfStructureViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow: Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private BackgroundWorker _worker;

		private void ParseFile(String filePath)
		{
			if (_worker == null)
			{
				_worker = new BackgroundWorker();
				_worker.WorkerReportsProgress = true;
				_worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
				_worker.DoWork += new DoWorkEventHandler(worker_DoWork);
				_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
				progressBar.Maximum = 100.0;
				progressBar.Value = 0;
				progressBar.Visibility = System.Windows.Visibility.Visible;
				_worker.RunWorkerAsync(filePath);
				parsingTime.Content = "Parsing PDF.... Please Wait...";
			}
			else
			{
				MessageBox.Show("Still loading another file, please wait...");
			}
		}

		private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar.Maximum = 100.0;
			progressBar.Value = e.ProgressPercentage;
		}

		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			PdfFile file = e.Result as PdfFile;
			fileView.ImageControl = image;

			fileView.FileStructure = file;
			documentView.DocumentStructure = new PdfDocument(file);

			progressBar.Visibility = Visibility.Collapsed;
			_worker = null;
			parsingTime.Content = String.Format("File parsed in {0:0.0} seconds", file.ParsingTime);
		}

		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			var file = PdfFile.Parse(e.Argument as String, Parse_Progress);
			e.Result = file;
		}

		private void Parse_Progress(object source, ProgressChangedEventArgs e)
		{
			_worker.ReportProgress(e.ProgressPercentage);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

			dlg.DefaultExt = ".pdf";
			dlg.Filter = "PDF documents (.pdf)|*.pdf";

			Nullable<bool> result = dlg.ShowDialog();
			if (result == true)
			{
				string fullPath = dlg.FileName;
				ParseFile(fullPath);
			}
		}

	}
}
