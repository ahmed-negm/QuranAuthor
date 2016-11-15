using QuranAuthor.Helps;
using QuranAuthor.Models;
using QuranAuthor.Repositories;
using QuranAuthor.Services;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace QuranAuthor.Views
{
    public partial class SnippetWindow : Window
    {
        private ClipboardHelper clipboardHelper;
        private SnippetService snippetService = new SnippetService();
        private SnippetRepository snippetRepository = new SnippetRepository();
        private Bitmap originalPage;
        private bool suspendEvents = false;

        public Bitmap Page { get; set; }
        public Snippet Snippet { get; set; }

        public SnippetWindow()
        {
            InitializeComponent();
            this.clipboardHelper = new ClipboardHelper(this);
            this.clipboardHelper.ItemCopied += ClipboardHelper_ItemCopied;
            grdControls.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.clipboardHelper.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.clipboardHelper.Start();
        }

        private void Point_ValueChanged(object sender, EventArgs e)
        {
            if (suspendEvents || Snippet == null)
            {
                return;
            }
            suspendEvents = true;
            Snippet.StartPoint = numStart.Value;
            Snippet.EndPoint = numEnd.Value;
            suspendEvents = false;
            RenderSelection();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (txtVerse != null && Snippet != null)
            {
                txtVerse.Text = Snippet.Text;
                txtVerse.IsEnabled = chkPart.IsChecked.Value;
                btnDone.IsEnabled = !chkPart.IsChecked.Value;
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            this.Snippet.Text = txtVerse.Text.Trim();
            this.DialogResult = true;
        }

        private void TxtVerse_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnDone.IsEnabled = true;
        }

        private void ClipboardHelper_ItemCopied(object sender, ItemCopiedEventArgs e)
        {
            Snippet = snippetService.ExtractSnippet(e.Rtf);

            var bitmap = WindowCapturer.Capture();

            var selection = BitmapHelper.GetSnippetSelection(bitmap);

            Snippet = BitmapHelper.CalculatePageSelection(Snippet, selection);

            LoadImage();
            RenderSelection();
            txtVerse.Text = Snippet.Text;

            suspendEvents = true;
            numStart.Value = Snippet.StartPoint;
            numEnd.Value = Snippet.EndPoint;
            suspendEvents = false;
            btnDone.IsEnabled = false;
            grdControls.Visibility = System.Windows.Visibility.Visible;
            this.Activate();
        }

        private void LoadImage()
        {
            var originalBmp = new Bitmap(@"E:\Fun\Tafseer\Images\Nexus 9\final\" + Snippet.Page + ".png");
            var rect = new System.Drawing.Rectangle(0, 0, originalBmp.Width, originalBmp.Height);

            originalPage = originalBmp.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            originalBmp.Dispose();
        }

        private void RenderSelection()
        {
            this.Page = BitmapHelper.FocusSelection((Bitmap)originalPage.Clone(), Snippet);
            imgPage.Source = BitmapHelper.BitmapToImageSource(this.Page);
        }
    }
}