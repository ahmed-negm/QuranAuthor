using QuranAuthor.Helps;
using QuranAuthor.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace QuranAuthor.Views
{
    public partial class TafseerWindow : Window
    {
        private bool suspendExplanationTopEvent = false;

        public TafseerWindow()
        {
            InitializeComponent();
            this.ViewModel = new TafseerViewModel();
            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.numExpTop.ValueChanged += numExpTop_ValueChanged;
        }

        public TafseerViewModel ViewModel
        {
            get
            {
                return this.DataContext as TafseerViewModel;
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
                this.ViewModel.SnippetTaken(snipetWindow.Snippet, snipetWindow.Page);
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            this.ViewModel.SaveExplanation();
        }

        private void numExpTop_ValueChanged(object sender, System.EventArgs e)
        {
            this.suspendExplanationTopEvent = true;
            this.ViewModel.ExplanationTop = this.numExpTop.Value;
            this.suspendExplanationTopEvent = false;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ExplanationTop" && !this.suspendExplanationTopEvent)
            {
                this.numExpTop.Value = this.ViewModel.ExplanationTop;
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
