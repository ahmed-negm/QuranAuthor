using QuranAuthor.Helps;
using QuranAuthor.Repositories;
using QuranAuthor.Services;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace QuranAuthor
{

    public partial class SnippetWindow : Window
    {

        private ClipboardHelper clipboardHelper;
        private SnippetService snippetService = new SnippetService();
        private SnippetRepository snippetRepository = new SnippetRepository();

        public SnippetWindow()
        {
            InitializeComponent();
            this.clipboardHelper = new ClipboardHelper(this);
            this.clipboardHelper.ItemCopied += ClipboardHelper_ItemCopied;
        }

        private void ClipboardHelper_ItemCopied(object sender, ItemCopiedEventArgs e)
        {
            var snippet = snippetService.ExtractSnippet(e.Rtf);

            var bitmap = WindowCapturer.Capture();

            var selection = BitmapHelper.GetSnippetSelection(bitmap);

            snippet = BitmapHelper.CalculatePageSelection(snippet, selection);

            //snippetRepository.AddSnippet(snippet);

            var bmp = new Bitmap(@"E:\Fun\Tafseer\Images\Nexus 9\final\" + snippet.Page + ".png");
            var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);

            var upgradedBmp = bmp.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmp.Dispose();

            Bitmap page = BitmapHelper.FocusSelection(upgradedBmp, snippet);

            //page.Save("E://page.png", System.Drawing.Imaging.ImageFormat.Png);

            imgPage.Source = BitmapToImageSource(page);
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
    }
}