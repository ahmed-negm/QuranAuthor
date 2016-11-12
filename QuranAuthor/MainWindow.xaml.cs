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
            snipetWindow.Show();
        }
    }
}