using QuranAuthor.ViewModels;
using System.ComponentModel;
using System.Configuration;
using System.Windows;

namespace QuranAuthor.Views
{
    public partial class TafseerWindow : Window
    {
        

        private bool suspendEvents = false;

        public TafseerWindow()
        {
            InitializeComponent();
            this.ViewModel = new TafseerViewModel();
            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.numExpTop.ValueChanged += numExpTop_ValueChanged;
            this.numPage.ValueChanged += numPage_ValueChanged;

            int defaultChapter = 1;
            int.TryParse(ConfigurationManager.AppSettings["TafseerDefaultChapter"], out defaultChapter);

            int defaultPage = 1;
            int.TryParse(ConfigurationManager.AppSettings["TafseerDefaultPage"], out defaultPage);

            this.ViewModel.Chapter = this.ViewModel.Chapters[defaultChapter - 1];
            this.ViewModel.CurrentPage = defaultPage;
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
                this.ViewModel.SnippetTaken(snipetWindow.Snippet);
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            this.ViewModel.SaveExplanation();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SnippetWindow snipetWindow = new SnippetWindow();
            snipetWindow.NewSnippet += SnipetWindow_NewSnippet;
            snipetWindow.Show();
            this.Activate();
        }

        private void SnipetWindow_NewSnippet(object sender, NewSnippetEventArgs e)
        {
            if(e.Snippet.Top < 0 || e.Snippet.StartPoint < 0 || e.Snippet.EndPoint < 0)
            {
                return;
            }
            if (this.ViewModel.Explanation != null)
            {
                this.ViewModel.ExplanationText = this.txtExp.Text;
            }
            this.ViewModel.SnippetTaken(e.Snippet);
            this.Activate();
            this.NewExp_Click(null, null);
        }

        private void numExpTop_ValueChanged(object sender, System.EventArgs e)
        {
            this.suspendEvents = true;
            this.ViewModel.ExplanationTop = this.numExpTop.Value;
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
        }

        private void NewExp_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.NewExpCommand.Execute(null);
            txtExp.SelectAll();
            txtExp.Focus();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.ViewModel.Snippet == null)
            {
                return;
            }
            SnippetWindow snipetWindow = new SnippetWindow();
            snipetWindow.Snippet = this.ViewModel.Snippet;
            snipetWindow.LoadSnippet();
            if (snipetWindow.ShowDialog() == true && snipetWindow.Snippet != null)
            {
                this.ViewModel.SnippetUpdated(snipetWindow.Snippet);
            }
        }
    }
}