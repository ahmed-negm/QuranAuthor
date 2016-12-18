using QuranAuthor.Commands;
using QuranAuthor.Helps;
using QuranAuthor.Models;
using QuranAuthor.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuranAuthor.ViewModels
{
    public class GenSimilarViewModel : ViewModelBase
    {
        private ChapterRepository chapterRepository = new ChapterRepository();
        private SimilarSnippetsRepository snippetRepository = new SimilarSnippetsRepository();
        private ExplanationRepository explanationRepository = new ExplanationRepository();

        private Chapter startChapter;
        private Chapter endChapter;
        private bool multiChapters;
        private bool isIdle = true;
        private string genPath = "E:\\Fun\\Tafseer\\Output\\Similar";

        private DelegateCommand genCommand;

        public GenSimilarViewModel()
        {
            this.StartChapter = this.Chapters[38];
            this.EndChapter = this.Chapters[38];
        }

        public Chapter StartChapter
        {
            get { return this.startChapter; }
            set
            {
                this.startChapter = value;
                base.OnPropertyChanged("StartChapter");
            }
        }

        public Chapter EndChapter
        {
            get { return this.endChapter; }
            set
            {
                this.endChapter = value;
                base.OnPropertyChanged("EndChapter");
            }
        }

        public bool MultiChapters
        {
            get { return this.multiChapters; }
            set
            {
                this.multiChapters = value;
                base.OnPropertyChanged("MultiChapters");
            }
        }

        public bool IsIdle
        {
            get { return this.isIdle; }
            set
            {
                this.isIdle = value;
                base.OnPropertyChanged("IsIdle");
            }
        }

        public string GenPath
        {
            get { return this.genPath; }
            set
            {
                this.genPath = value;
                base.OnPropertyChanged("GenPath");
            }
        }

        public List<Chapter> Chapters
        {
            get
            {
                return this.chapterRepository.GetChapters();
            }
        }

        public ICommand GenCommand
        {
            get
            {
                if (genCommand == null)
                {
                    genCommand = new DelegateCommand(Generate, CanGenerate);
                }
                return genCommand;
            }
        }

        private bool CanGenerate()
        {
            if(this.MultiChapters == false)
            {
                return true;
            }
            else
            {
                return this.StartChapter.Id <= this.EndChapter.Id;
            }
        }
        
        private void Generate()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            this.IsIdle = false;
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var startChapter = this.StartChapter.Id;
            var endChapter = this.EndChapter.Id;
            if (this.MultiChapters == false)
            {
                endChapter = startChapter;
            }

            for (int c = startChapter; c <= endChapter; c++)
            {
                var chapterPath = Path.Combine(this.GenPath, c.ToString());
                int pageIndex = 0;
                this.RefreshFolder(chapterPath);
                var startPage = this.Chapters[c - 1].StartPage;
                var endPage = this.Chapters[(c == 114 ? 114 : c)].StartPage;
                for (int p = startPage; p <= endPage; p++)
                {
                    var snippets = this.snippetRepository.GetSnippets(c, p);
                    if (snippets.Count == 0)
                    {
                        continue;
                    }

                    var page = BitmapHelper.LoadPage(p);
                    foreach (var snippet in snippets)
                    {
                        var explanations = this.explanationRepository.GetExplanations(snippet.Id);
                        var similarSnippets = this.snippetRepository.GetSnippetsByParentId(snippet.Id);

                        var snippetPage = BitmapHelper.FocusSelection((Bitmap)page.Clone(), snippet);
                        snippetPage = BitmapHelper.DrawSmiliarExplanation(snippetPage, explanations);
                        snippetPage = BitmapHelper.DrawSimilarSnippets(snippetPage, similarSnippets);
                        snippetPage.Save(Path.Combine(chapterPath, this.GetFileName(pageIndex) + ".png"));
                        pageIndex++;
                    }
                }
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.IsIdle = true;
            UIHelper.MessageBox("تم التخريج بنجاح");
        }

        private string GetFileName(int pageIndex)
        {
            if (pageIndex < 10)
            {
                return "000" + pageIndex.ToString();
            }

            if (pageIndex < 100)
            {
                return "00" + pageIndex.ToString();
            }

            if (pageIndex < 1000)
            {
                return "0" + pageIndex.ToString();
            }

            return pageIndex.ToString();
        }

        private void RefreshFolder(string p)
        {
            try
            {
                Directory.Delete(p);
            }
            catch { }
            Directory.CreateDirectory(p);
        }
    }
}
