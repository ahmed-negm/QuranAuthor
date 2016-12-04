using QuranAuthor.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace QuranAuthor.Views
{
    public partial class SimilarWindow : Window
    {
        private bool suspendEvents = false;

        public SimilarWindow()
        {
            InitializeComponent();
            this.ViewModel = new SimilarViewModel();
            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.numExpTop.ValueChanged += numExpTop_ValueChanged;
            this.numPage.ValueChanged += numPage_ValueChanged;

            this.ViewModel.Chapter = this.ViewModel.Chapters[38];
        }

        public SimilarViewModel ViewModel
        {
            get
            {
                return this.DataContext as SimilarViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        private void NewSnippet_Click(object sender, RoutedEventArgs e)
        {
            SnippetWindow snipetWindow = new SnippetWindow();
            if (snipetWindow.ShowDialog() == true && snipetWindow.Snippet != null)
            {
                this.ViewModel.SnippetTaken(snipetWindow.Snippet);
            }
        }

        private void NewSimilarSnippet_Click(object sender, RoutedEventArgs e)
        {
            SnippetWindow snipetWindow = new SnippetWindow();
            if (snipetWindow.ShowDialog() == true && snipetWindow.Snippet != null)
            {
                this.ViewModel.SimilarSnippetTaken(snipetWindow.Snippet);
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            this.ViewModel.SaveExplanation();
            this.ViewModel.SaveSimilarSnippet();
        }

        private void numExpTop_ValueChanged(object sender, System.EventArgs e)
        {
            this.suspendEvents = true;
            this.ViewModel.ExplanationTop = this.numExpTop.Value;
            this.suspendEvents = false;
        }

        private void numSimilarTop_ValueChanged(object sender, System.EventArgs e)
        {
            this.suspendEvents = true;
            this.ViewModel.SimilarTop = this.numSimilarTop.Value;
            this.suspendEvents = false;
        }

        private void numPage_ValueChanged(object sender, System.EventArgs e)
        {
            this.suspendEvents = true;
            this.ViewModel.CurrentPage = this.numPage.Value;
            this.suspendEvents = false;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.suspendEvents)
            {
                return;
            }

            if (e.PropertyName == "ExplanationTop")
            {
                this.numExpTop.Value = this.ViewModel.ExplanationTop;
            }
            else if (e.PropertyName == "CurrentPage")
            {
                this.numPage.Value = this.ViewModel.CurrentPage;
            }
            else if (e.PropertyName == "SimilarTop")
            {
                this.numSimilarTop.Value = this.ViewModel.SimilarTop;
            }
        }

        private void NewExp_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.NewExpCommand.Execute(null);
            txtExp.SelectAll();
            txtExp.Focus();
        }
    }
}