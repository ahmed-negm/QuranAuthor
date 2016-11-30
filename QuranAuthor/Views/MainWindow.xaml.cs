using System.Windows;

namespace QuranAuthor.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TafseerEnter_Click(object sender, RoutedEventArgs e)
        {
            var window = new TafseerWindow();
            window.ShowDialog();
        }

        private void TafseerGen_Click(object sender, RoutedEventArgs e)
        {
            var window = new GenTafseerWindow();
            window.ShowDialog();
        }
    }
}