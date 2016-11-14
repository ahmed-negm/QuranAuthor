using QuranAuthor.Helps;
using QuranAuthor.ViewModels;
using System.Windows;

namespace QuranAuthor.Views
{
    public partial class TafseerWindow : Window
    {
        public TafseerWindow()
        {
            InitializeComponent();
            this.DataContext = new TafseerViewModel();
        }

        private void takeSnippet_Click(object sender, RoutedEventArgs e)
        {
            SnippetWindow snipetWindow = new SnippetWindow();
            snipetWindow.ShowDialog();
            imgPage.Source = BitmapHelper.BitmapToImageSource(snipetWindow.Page);
            var snippet = snipetWindow.Snippet;
        }
    }
}
