using QuranAuthor.Helps;
using QuranAuthor.Models;
using QuranAuthor.Repositories;
using QuranAuthor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private Bitmap originalPage;
        private bool suspendEvents = false;
        public Bitmap Page { get; set; }
        public Snippet Snippet { get; set; }
        public ObservableCollection<SnippetMark> Marks { get; set; }
        public SnippetMark Mark { get; set; }

        public SnippetWindow()
        {
            InitializeComponent();
            this.clipboardHelper = new ClipboardHelper(this);
            this.clipboardHelper.ItemCopied += ClipboardHelper_ItemCopied;
            this.Marks = new ObservableCollection<SnippetMark>();
            this.marksLst.ItemsSource = this.Marks;
            tabControl.Visibility = System.Windows.Visibility.Hidden;
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
            tabControl.Visibility = System.Windows.Visibility.Visible;
            this.Activate();
        }

        private void LoadImage()
        {
            originalPage = BitmapHelper.LoadPage(Snippet.Page);
        }

        private void RenderSelection()
        {
            this.Page = BitmapHelper.FocusSelection((Bitmap)originalPage.Clone(), Snippet);
            imgPage.Source = BitmapHelper.BitmapToImageSource(this.Page);
        }

        private void Mark_ValueChanged(object sender, EventArgs e)
        {
            var index = this.marksLst.SelectedIndex;
            if (index < 0 || this.suspendEvents)
            {
                return;
            }
            this.Marks[index].Line = this.markLine.Value;
            this.Marks[index].StartPoint = this.markStart.Value;
            this.Marks[index].EndPoint = this.markEnd.Value;
        }

        private void NewMark_Click(object sender, RoutedEventArgs e)
        {
            var mark = new SnippetMark();
            mark.SnippetId = this.Snippet.Id;
            this.Marks.Add(mark);
            this.marksLst.SelectedIndex = this.Marks.Count - 1;
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
                this.Marks.RemoveAt(index);
            }
        }

        private void UpMark_Click(object sender, RoutedEventArgs e)
        {
            var index = this.marksLst.SelectedIndex;
            if (index <= 0)
            {
                return;
            }
            var selected = this.Marks[index];
            this.Marks[index] = this.Marks[index - 1];
            this.Marks[index - 1] = selected;
            this.marksLst.SelectedIndex = index - 1;
        }

        private void DownMark_Click(object sender, RoutedEventArgs e)
        {
            var index = this.marksLst.SelectedIndex;
            if(index >= this.Marks.Count - 1)
            {
                return;
            }
            var selected = this.Marks[index];
            this.Marks[index] = this.Marks[index + 1];
            this.Marks[index + 1] = selected;
            this.marksLst.SelectedIndex = index + 1;
        }

        private void MarkList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = this.marksLst.SelectedIndex;
            if (index < 0)
            {
                return;
            }
            this.suspendEvents = true;
            this.markLine.Value = this.Marks[index].Line;
            this.markStart.Value = this.Marks[index].StartPoint;
            this.markEnd.Value = this.Marks[index].EndPoint;
            this.suspendEvents = false;
        }
    }
}