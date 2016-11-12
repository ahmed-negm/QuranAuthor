using QuranAuthor.Helps;
using System.Windows;

namespace QuranAuthor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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