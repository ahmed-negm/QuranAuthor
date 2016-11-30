using QuranAuthor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuranAuthor.Views
{
    /// <summary>
    /// Interaction logic for GenTafseerWindow.xaml
    /// </summary>
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
