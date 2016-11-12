using QuranAuthor.Helps;
using QuranAuthor.Models;
using QuranAuthor.Repositories;
using QuranAuthor.Services;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace QuranAuthor
{

    public partial class SnippetWindow : Window
    {

        private ClipboardHelper clipboardHelper;
        private SnippetService snippetService = new SnippetService();
        private SnippetRepository snippetRepository = new SnippetRepository();
        private Snippet snippet;
        private Bitmap page;
        private bool suspendEvents = false;

        public SnippetWindow()
        {
            InitializeComponent();
            this.clipboardHelper = new ClipboardHelper(this);
            this.clipboardHelper.ItemCopied += ClipboardHelper_ItemCopied;
        }

        private void ClipboardHelper_ItemCopied(object sender, ItemCopiedEventArgs e)
        {
            snippet = snippetService.ExtractSnippet(e.Rtf);

            var bitmap = WindowCapturer.Capture();

            var selection = BitmapHelper.GetSnippetSelection(bitmap);

            snippet = BitmapHelper.CalculatePageSelection(snippet, selection);

            LoadImage();
            renderSelection();
            txtVerse.Text = snippet.Text;

            suspendEvents = true;
            numStart.Value = snippet.StartPoint;
            numEnd.Value = snippet.EndPoint;
            suspendEvents = false;
        }

        private void LoadImage()
        {
            var originalBmp = new Bitmap(@"E:\Fun\Tafseer\Images\Nexus 9\final\" + snippet.Page + ".png");
            var rect = new System.Drawing.Rectangle(0, 0, originalBmp.Width, originalBmp.Height);

            page = originalBmp.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            originalBmp.Dispose();
        }

        private void renderSelection()
        {
            if (suspendEvents)
            {
                return;
            }

            imgPage.Source = BitmapToImageSource(BitmapHelper.FocusSelection((Bitmap)page.Clone(), snippet));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.clipboardHelper.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.clipboardHelper.Start();
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void point_ValueChanged(object sender, EventArgs e)
        {
            if(snippet != null)
            {
                suspendEvents = true;
                snippet.StartPoint = numStart.Value;
                snippet.EndPoint = numEnd.Value;
                suspendEvents = false;
                renderSelection();
            }
        }
    }
}