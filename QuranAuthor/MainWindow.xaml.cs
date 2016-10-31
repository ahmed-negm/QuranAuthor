using QuranAuthor.Helps;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuranAuthor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClipboardHelper clipboardHelper;

        public MainWindow()
        {
            InitializeComponent();
            this.clipboardHelper = new ClipboardHelper(this);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.clipboardHelper.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.clipboardHelper.Start();
        }
    }
}
