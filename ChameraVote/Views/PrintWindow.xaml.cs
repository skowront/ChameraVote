using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using ChameraVote.ViewModels;
using ChameraVote.Pdf;
using MigraDoc.Rendering;

namespace ChameraVote.Views
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        public PrintViewModel PrintViewModel = null;

        public PrintWindow(VotingSumResultsViewModel votingSumResultsViewModel )
        {
            this.PrintViewModel = new PrintViewModel(votingSumResultsViewModel);
            InitializeComponent();
            this.DataContext = this.PrintViewModel;
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; 
            dlg.DefaultExt = ".pdf"; 
            dlg.Filter = "Pdf documents (.pdf)|*.pdf";
            dlg.CheckFileExists = false;

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                PdfFactory factory = new PdfFactory();
                var document = factory.BuildVotingResultsPdf(this.PrintViewModel.VotingSumResultsViewModel,this.LanguageList.SelectedItem?.ToString()); ;
                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
                renderer.Document = document;
                renderer.RenderDocument();
                renderer.PdfDocument.Save(dlg.FileName);
                
            }
        }
    }
}
