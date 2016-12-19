using QuranAuthor.ViewModels;
using System.Windows;

namespace QuranAuthor.Views
{
    public partial class GenSimilarExercisesWindow : Window
    {
        public GenSimilarExercisesWindow()
        {
            InitializeComponent();
            this.ViewModel = new GenSimilarExercisesViewModel();
        }

        public GenSimilarExercisesViewModel ViewModel
        {
            get
            {
                return this.DataContext as GenSimilarExercisesViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}