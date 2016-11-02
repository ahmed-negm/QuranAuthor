using QuranAuthor.Helps;
using QuranAuthor.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            this.clipboardHelper.ItemCopied += ClipboardHelper_ItemCopied;
        }

        private void ClipboardHelper_ItemCopied(object sender, ItemCopiedEventArgs e)
        {
            var bitmap = WindowCapturer.Capture();

            var selection = BitmapHelper.GetSnippetSelection(bitmap);

            var snippet = BitmapHelper.CalculatePageSelection(selection);

            var bmp = new Bitmap(@"E:\Fun\Tafseer\Images\Nexus 9\final\459.png");
            var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);

            var upgradedBmp = bmp.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmp.Dispose();

            Bitmap page = BitmapHelper.FocusSelection(upgradedBmp, snippet);

            page.Save("E://page.png", System.Drawing.Imaging.ImageFormat.Png);
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
