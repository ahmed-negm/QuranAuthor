using QuranAuthor.Commands;
using QuranAuthor.Helps;
using QuranAuthor.Models;
using QuranAuthor.Repositories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
namespace QuranAuthor.ViewModels
{
    public class TafseerViewModel : ViewModelBase
    {
        #region Private Members

        // Services
        private ChapterRepository chapterRepository = new ChapterRepository();
        private SnippetRepository snippetRepository = new SnippetRepository();
        private ExplanationRepository explanationRepository = new ExplanationRepository();

        // Private properties
        private Chapter chapter;
        private Snippet snippet;
        private bool hasSnippet;
        private bool hasExplanation;
        private Explanation explanation;
        private int explanationType;
        private int explanationTop;
        private string explanationText;
        private bool suspendExplanationEvents;
        private bool isEditMode;
        private ImageSource imageSource;

        // Commands
        private DelegateCommand deleteCommand;
        private DelegateCommand upCommand;
        private DelegateCommand downCommand;
        private DelegateCommand deleteExpCommand;
        private DelegateCommand upExpCommand;
        private DelegateCommand downExpCommand;
        private DelegateCommand newExpCommand;
        private DelegateCommand switchModeCommand;
        
        #endregion

        #region Constructor

        public TafseerViewModel()
        {
            this.Snippets = new ObservableCollection<Snippet>();
            this.Explanations = new ObservableCollection<Explanation>();
            this.Chapter = this.Chapters[38];
            this.IsEditMode = true;
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
                this.LoadExplanations();
            }
        }

        public Explanation Explanation
        {
            get { return this.explanation; }
            set
            {
                this.SaveExplanation();
                this.explanation = value;
                base.OnPropertyChanged("Explanation");
                this.LoadExplanation();
            }
        }

        public ImageSource ImageSource
        {
            get { return this.imageSource; }
            set
            {
                this.imageSource = value;
                base.OnPropertyChanged("ImageSource");
            }
        }

        public bool HasSnippet
        {
            get { return this.hasSnippet; }
            set
            {
                this.hasSnippet = value;
                base.OnPropertyChanged("HasSnippet");
            }
        }

        public bool HasExplanation
        {
            get { return this.hasExplanation; }
            set
            {
                this.hasExplanation = value;
                base.OnPropertyChanged("HasExplanation");
            }
        }

        public bool IsEditMode
        {
            get { return this.isEditMode; }
            set
            {
                this.isEditMode = value;
                base.OnPropertyChanged("IsEditMode");
            }
        }

        public int ExplanationType
        {
            get { return this.explanationType; }
            set
            {
                this.explanationType = value;
                base.OnPropertyChanged("ExplanationType");
                this.Explanation.Type = (ExplanationType)value;
                this.Explanation.RaisePropertyChanged("Type");
                this.SaveExplanation();
            }
        }

        public int ExplanationTop
        {
            get { return this.explanationTop; }
            set
            {
                this.explanationTop = value;
                base.OnPropertyChanged("ExplanationTop");
                this.Explanation.Top = value;
                this.Explanation.RaisePropertyChanged("Top");
                this.SaveExplanation();
            }
        }

        public string ExplanationText
        {
            get { return this.explanationText; }
            set
            {
                this.explanationText = value;
                base.OnPropertyChanged("ExplanationText");
                this.Explanation.Text = value;
                this.SaveExplanation();
                this.Explanation.RaisePropertyChanged("Text");
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

        public ObservableCollection<Explanation> Explanations { get; set; }

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

        public ICommand DeleteExpCommand
        {
            get
            {
                if (deleteExpCommand == null)
                {
                    deleteExpCommand = new DelegateCommand(DeleteExplanation, CanDeleteExplanation);
                }
                return deleteExpCommand;
            }
        }

        public ICommand UpExpCommand
        {
            get
            {
                if (upExpCommand == null)
                {
                    upExpCommand = new DelegateCommand(UpExplanation, CanUpExplanation);
                }
                return upExpCommand;
            }
        }

        public ICommand DownExpCommand
        {
            get
            {
                if (downExpCommand == null)
                {
                    downExpCommand = new DelegateCommand(DownExplanation, CanDownExplanation);
                }
                return downExpCommand;
            }
        }

        public ICommand NewExpCommand
        {
            get
            {
                if (newExpCommand == null)
                {
                    newExpCommand = new DelegateCommand(NewExplanation, CanNewExplanation);
                }
                return newExpCommand;
            }
        }

        public ICommand SwitchModeCommand
        {
            get
            {
                if (switchModeCommand == null)
                {
                    switchModeCommand = new DelegateCommand(SwitchMode, CanSwitchMode);
                }
                return switchModeCommand;
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

        public void SaveExplanation()
        {
            if (this.suspendExplanationEvents || this.Explanation == null)
            {
                return;
            }

            this.explanationRepository.Update(this.Explanation);
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

        private bool CanDeleteExplanation()
        {
            return this.Explanation != null;
        }

        private void DeleteExplanation()
        {
            this.suspendExplanationEvents = true;
            int index = this.Explanations.IndexOf(this.Explanation);
            this.explanationRepository.Delete(this.Explanation.Id);
            LoadExplanations();
            this.Explanation = this.Explanations.Count > index ? this.Explanations[index] : null;
            this.suspendExplanationEvents = false;
        }

        private bool CanUpExplanation()
        {
            return this.Explanation != null && this.explanation != this.Explanations[0];
        }

        private void UpExplanation()
        {
            this.suspendExplanationEvents = true;
            int index = this.Explanations.IndexOf(this.Explanation);
            this.explanationRepository.Swap(this.Explanation, this.Explanations[index - 1]);
            LoadExplanations();
            this.Explanation = this.Explanations[index - 1];
            this.LoadExplanation();
            this.suspendExplanationEvents = false;
        }

        private bool CanDownExplanation()
        {
            return this.Explanation != null && this.explanation != this.Explanations[this.Explanations.Count - 1];
        }

        private void DownExplanation()
        {
            this.suspendExplanationEvents = true;
            int index = this.Explanations.IndexOf(this.Explanation);
            this.explanationRepository.Swap(this.Explanation, this.Explanations[index + 1]);
            LoadExplanations();
            this.Explanation = this.Explanations[index + 1];
            this.LoadExplanation();
            this.suspendExplanationEvents = false;
        }

        private bool CanNewExplanation()
        {
            return this.snippet != null;
        }

        private void NewExplanation()
        {
            var newExplanation = new Explanation();
            newExplanation.SnippetId = this.Snippet.Id;
            newExplanation.Type = 0;
            newExplanation.Top = 0;
            newExplanation.Text = "-";
            this.explanationRepository.AddExplanation(newExplanation);
            this.LoadExplanations();
            this.Explanation = this.Explanations[this.Explanations.Count - 1];
        }

        private void LoadExplanations()
        {
            this.HasSnippet = this.Snippet != null;
            if (this.HasSnippet)
            {
                this.Explanations.Clear();
                var explanations = this.explanationRepository.GetExplanations(this.Snippet.Id);
                explanations.ForEach(S => this.Explanations.Add(S));
            }
        }

        private void LoadExplanation()
        {
            this.HasExplanation = this.Explanation != null;
            if(this.HasExplanation)
            {
                this.suspendExplanationEvents = true;
                this.ExplanationType = (int)this.Explanation.Type;
                this.ExplanationTop = this.Explanation.Top;
                this.ExplanationText = this.Explanation.Text;
                this.suspendExplanationEvents = false;
            }
        }

        private bool CanSwitchMode()
        {
            return this.snippet != null;
        }

        private void SwitchMode()
        {
            this.IsEditMode = !this.IsEditMode;
            if(!this.IsEditMode)
            {
                this.Page = BitmapHelper.LoadPage(this.Snippet.Page);
                this.Page = BitmapHelper.FocusSelection(this.Page, this.Snippet);
                Bitmap expPage = BitmapHelper.DrawExplanation((Bitmap)this.Page.Clone(), this.Explanations);
                expPage.Save("E:\\1.png");
                this.ImageSource = BitmapHelper.BitmapToImageSource(expPage);
            }
        }

        #endregion
    }
}