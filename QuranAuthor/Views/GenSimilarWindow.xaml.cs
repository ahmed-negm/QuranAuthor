using QuranAuthor.ViewModels;
using System.Windows;

namespace QuranAuthor.Views
{
    public partial class GenSimilarWindow : Window
    {
        public GenSimilarWindow()
        {
            InitializeComponent();
            this.ViewModel = new GenSimilarViewModel();
        }

        public GenSimilarViewModel ViewModel
        {
            get
            {
                return this.DataContext as GenSimilarViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}