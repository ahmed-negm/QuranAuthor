using QuranAuthor.ViewModels;
using System.Windows;

namespace QuranAuthor.Views
{
    public partial class GenTafseerWindow : Window
    {
        public GenTafseerWindow()
        {
            InitializeComponent();
            this.ViewModel = new GenTafseerViewModel();
        }

        public GenTafseerViewModel ViewModel
        {
            get
            {
                return this.DataContext as GenTafseerViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}