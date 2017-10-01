using QuranAuthor.Helps;
using QuranAuthor.Models;
using QuranAuthor.Services;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace QuranAuthor.Views
{
    public class NewSnippetEventArgs : EventArgs
    {
        public Snippet Snippet { get; set; }
    }

    public partial class SnippetWindow : Window
    {
        private ClipboardHelper clipboardHelper;
        private SnippetService snippetService = new SnippetService();
        private Bitmap originalPage;
        private bool suspendEvents = false;
        private Bitmap page;

        public Snippet Snippet { get; set; }

        public event EventHandler<NewSnippetEventArgs> NewSnippet;

        public SnippetWindow()
        {
            InitializeComponent();
            this.clipboardHelper = new ClipboardHelper(this);
            this.clipboardHelper.ItemCopied += ClipboardHelper_ItemCopied;
            this.tabControl.Visibility = System.Windows.Visibility.Hidden;
            this.marksgrd.Visibility = System.Windows.Visibility.Hidden;
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
            this.Snippet = snippetService.ExtractSnippet(e.Rtf);
            if(this.Snippet == null)
            {
                return;
            }
            this.Snippet.Marks = new ObservableCollection<SnippetMark>();
            this.marksLst.ItemsSource = this.Snippet.Marks;

            var bitmap = WindowCapturer.Capture();

            var selection = BitmapHelper.GetSnippetSelection(bitmap);

            Snippet = BitmapHelper.CalculatePageSelection(Snippet, selection);

            LoadSnippet();
            this.Activate();
            OnNewSnippet(this.Snippet);
        }

        public void LoadSnippet()
        {
            LoadImage();
            RenderSelection();
            txtVerse.Text = Snippet.Text;

            suspendEvents = true;
            numStart.Value = Snippet.StartPoint;
            numEnd.Value = Snippet.EndPoint;
            suspendEvents = false;
            btnDone.IsEnabled = false;
            tabControl.Visibility = System.Windows.Visibility.Visible;
        }

        private void LoadImage()
        {
            originalPage = BitmapHelper.LoadPage(Snippet.Page);
        }

        private void RenderSelection()
        {
            this.page = BitmapHelper.FocusSelection((Bitmap)originalPage.Clone(), Snippet);
            imgPage.Source = BitmapHelper.BitmapToImageSource(this.page);
        }

        private void Mark_ValueChanged(object sender, EventArgs e)
        {
            var index = this.marksLst.SelectedIndex;
            if (index < 0 || this.suspendEvents)
            {
                return;
            }
            this.Snippet.Marks[index].Line = this.markLine.Value;
            this.Snippet.Marks[index].StartPoint = this.markStart.Value;
            this.Snippet.Marks[index].EndPoint = this.markEnd.Value;
            this.RenderSelection();
        }

        private void NewMark_Click(object sender, RoutedEventArgs e)
        {
            var mark = new SnippetMark();
            mark.SnippetId = this.Snippet.Id;
            this.Snippet.Marks.Add(mark);
            this.marksLst.SelectedIndex = this.Snippet.Marks.Count - 1;
            this.RenderSelection();
        }

        private void DeleteMark_Click(object sender, RoutedEventArgs e)
        {
            var index = this.marksLst.SelectedIndex;
            if (index < 0)
            {
                return;
            }
            if (UIHelper.Ask("هل تريد حذف هذا العنصر؟"))
            {
                this.Snippet.Marks.RemoveAt(index);
            }
            this.RenderSelection();
        }

        private void UpMark_Click(object sender, RoutedEventArgs e)
        {
            var index = this.marksLst.SelectedIndex;
            if (index <= 0)
            {
                return;
            }
            var selected = this.Snippet.Marks[index];
            this.Snippet.Marks[index] = this.Snippet.Marks[index - 1];
            this.Snippet.Marks[index - 1] = selected;
            this.marksLst.SelectedIndex = index - 1;
        }

        private void DownMark_Click(object sender, RoutedEventArgs e)
        {
            var index = this.marksLst.SelectedIndex;
            if (index >= this.Snippet.Marks.Count - 1)
            {
                return;
            }
            var selected = this.Snippet.Marks[index];
            this.Snippet.Marks[index] = this.Snippet.Marks[index + 1];
            this.Snippet.Marks[index + 1] = selected;
            this.marksLst.SelectedIndex = index + 1;
        }

        private void MarkList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = this.marksLst.SelectedIndex;
            if (index < 0)
            {
                this.marksgrd.Visibility = System.Windows.Visibility.Hidden;
                return;
            }
            this.suspendEvents = true;
            this.markLine.Value = this.Snippet.Marks[index].Line;
            this.markStart.Value = this.Snippet.Marks[index].StartPoint;
            this.markEnd.Value = this.Snippet.Marks[index].EndPoint;
            this.suspendEvents = false;
            this.marksgrd.Visibility = System.Windows.Visibility.Visible;
        }

        protected virtual void OnNewSnippet(Snippet snippet)
        {
            var e = new NewSnippetEventArgs();
            e.Snippet = snippet;
            EventHandler<NewSnippetEventArgs> handler = NewSnippet;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}