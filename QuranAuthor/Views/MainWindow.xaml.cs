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

        private void SimilarEnter_Click(object sender, RoutedEventArgs e)
        {
            var window = new SimilarWindow();
            window.ShowDialog();
        }

        private void SimilarGen_Click(object sender, RoutedEventArgs e)
        {
            var window = new GenSimilarWindow();
            window.ShowDialog();
        }

        private void SimilarExercisesGen_Click(object sender, RoutedEventArgs e)
        {
            var window = new GenSimilarExercisesWindow();
            window.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}