using QuranAuthor.Commands;
using QuranAuthor.Helps;
using QuranAuthor.Models;
using QuranAuthor.Repositories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
namespace QuranAuthor.ViewModels
{
    public class TafseerViewModel : ViewModelBase
    {
        #region Private Members

        // Services
        private ChapterRepository chapterRepository = new ChapterRepository();
        private SnippetRepository snippetRepository = new SnippetRepository();

        // Private properties
        private Chapter chapter;
        private Snippet snippet;


        // Commands
        private DelegateCommand deleteCommand;
        private DelegateCommand upCommand;
        private DelegateCommand downCommand;

        #endregion

        #region Constructor

        public TafseerViewModel()
        {
            this.Snippets = new ObservableCollection<Snippet>();
            this.Chapter = this.Chapters[38];
        }

        #endregion

        #region Public Properties

        public Chapter Chapter
        {
            get { return this.chapter; }
            set
            {
                this.chapter = value;
                base.OnPropertyChanged("Chapter");
                this.LoadSnippets();
            }
        }

        public Snippet Snippet
        {
            get { return this.snippet; }
            set
            {
                this.snippet = value;
                base.OnPropertyChanged("Snippet");
            }
        }

        public List<Chapter> Chapters
        {
            get
            {
                return this.chapterRepository.GetChapters();
            }
        }

        public ObservableCollection<Snippet> Snippets { get; set; }

        public Bitmap Page { get; set; }

        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                {
                    deleteCommand = new DelegateCommand(DeleteSnippet, CanDeleteSnippet);
                }
                return deleteCommand;
            }
        }

        public ICommand UpCommand
        {
            get
            {
                if (upCommand == null)
                {
                    upCommand = new DelegateCommand(UpSnippet, CanUpSnippet);
                }
                return upCommand;
            }
        }

        public ICommand DownCommand
        {
            get
            {
                if (downCommand == null)
                {
                    downCommand = new DelegateCommand(DownSnippet, CanDownSnippet);
                }
                return downCommand;
            }
        }

        #endregion

        #region Public Methods

        public void SnippetTaken(Snippet snippet, Bitmap page)
        {
            if (snippet.ChapterId != this.Chapter.Id)
            {
                MessageBoxHelper.Show("The snippet belong to different chapter.");
                return;
            }

            this.Page = page;
            this.snippetRepository.AddSnippet(snippet);
            this.LoadSnippets();
            this.Snippet = this.Snippets[this.Snippets.Count - 1];
        }

        #endregion

        #region Private Methods

        private bool CanDeleteSnippet()
        {
            return this.Snippet != null;
        }

        private void DeleteSnippet()
        {
            int index = this.Snippets.IndexOf(this.Snippet);
            this.snippetRepository.Delete(this.Snippet.Id);
            LoadSnippets();
            this.Snippet = this.Snippets.Count > index ? this.Snippets[index] : null;
        }

        private bool CanUpSnippet()
        {
            return this.Snippet != null && this.snippet != this.Snippets[0];
        }

        private void UpSnippet()
        {
            int index = this.Snippets.IndexOf(this.Snippet);
            this.snippetRepository.Swap(this.Snippet, this.Snippets[index - 1]);
            LoadSnippets();
            this.Snippet = this.Snippets[index - 1];
        }

        private bool CanDownSnippet()
        {
            return this.Snippet != null && this.snippet != this.Snippets[this.Snippets.Count - 1];
        }

        private void DownSnippet()
        {
            int index = this.Snippets.IndexOf(this.Snippet);
            this.snippetRepository.Swap(this.Snippet, this.Snippets[index + 1]);
            LoadSnippets();
            this.Snippet = this.Snippets[index + 1];
        }

        private void LoadSnippets()
        {
            this.Snippets.Clear();
            var snippets = this.snippetRepository.GetSnippets(this.chapter.Id);
            snippets.ForEach(S => this.Snippets.Add(S));
        }

        #endregion
    }
}