using QuranAuthor.Commands;
using QuranAuthor.Helps;
using QuranAuthor.Models;
using QuranAuthor.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows.Input;
using System.Windows.Media;

namespace QuranAuthor.ViewModels
{
    public class SimilarViewModel : ViewModelBase
    {
        #region Private Members

        // Services
        private ChapterRepository chapterRepository = new ChapterRepository();
        private SimilarSnippetsRepository snippetRepository = new SimilarSnippetsRepository();
        private ExplanationRepository explanationRepository = new ExplanationRepository();

        // Private properties
        private Chapter chapter;
        private Snippet snippet;
        private Snippet similarSnippet;
        private bool hasSnippet;
        private bool hasExplanation;
        private Explanation explanation;
        private int explanationType;
        private int explanationTop;
        private int similarTop;
        private string explanationText;
        private bool suspendEvents;
        private bool isEditMode;
        private ImageSource imageSource;
        private int iconIndex;
        private bool hasIcon;
        private int currentPage;

        // Commands
        private DelegateCommand deleteCommand;
        private DelegateCommand upCommand;
        private DelegateCommand downCommand;

        private DelegateCommand newExpCommand;
        private DelegateCommand deleteExpCommand;
        private DelegateCommand upExpCommand;
        private DelegateCommand downExpCommand;
        private DelegateCommand exportExpCommand;
        private DelegateCommand importExpCommand;

        private DelegateCommand deleteSimilarCommand;
        private DelegateCommand upSimilarCommand;
        private DelegateCommand downSimilarCommand;
        private DelegateCommand exportSimilarCommand;
        private DelegateCommand importSimilarCommand;

        #endregion

        #region Constructor

        public SimilarViewModel()
        {
            this.Snippets = new ObservableCollection<Snippet>();
            this.Explanations = new ObservableCollection<Explanation>();
            this.SimilarSnippets = new ObservableCollection<Snippet>();
            this.Icons = new ObservableCollection<string>();
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
                this.CurrentPage = this.Chapter.StartPage;
            }
        }

        public Snippet Snippet
        {
            get { return this.snippet; }
            set
            {
                this.snippet = value;
                base.OnPropertyChanged("Snippet");
                this.LoadSnippet();
            }
        }

        public Snippet SimilarSnippet
        {
            get { return this.similarSnippet; }
            set
            {
                this.similarSnippet = value;
                base.OnPropertyChanged("SimilarSnippet");
                this.LoadSimilarSnippet();
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
                this.HasIcon = this.explanation.Type != Models.ExplanationType.Explain;
                this.Explanation.RaisePropertyChanged("Type");
                this.SaveExplanation();
            }
        }

        public int CurrentPage
        {
            get { return this.currentPage; }
            set
            {
                this.currentPage = value;
                base.OnPropertyChanged("CurrentPage");
                this.LoadSnippets();
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

        public int SimilarTop
        {
            get { return this.similarTop; }
            set
            {
                this.similarTop = value;
                base.OnPropertyChanged("SimilarTop");
                this.SimilarSnippet.Top = value;
                this.SaveSimilarSnippet();
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

        public int IconIndex
        {
            get { return this.iconIndex; }
            set
            {
                this.iconIndex = value;
                base.OnPropertyChanged("IconIndex");
                if (value >= 0)
                {
                    this.Explanation.Icon = value;
                    this.Explanation.RaisePropertyChanged("Icon");
                    this.SaveExplanation();
                }
            }
        }

        public bool HasIcon
        {
            get { return this.hasIcon; }
            set
            {
                this.hasIcon = value;
                base.OnPropertyChanged("HasIcon");
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

        public ObservableCollection<Snippet> SimilarSnippets { get; set; }

        public ObservableCollection<string> Icons { get; set; }

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

        public ICommand ExportExpCommand
        {
            get
            {
                if (exportExpCommand == null)
                {
                    exportExpCommand = new DelegateCommand(ExportExplanation, CanExportExplanation);
                }
                return exportExpCommand;
            }
        }

        public ICommand ImportExpCommand
        {
            get
            {
                if (importExpCommand == null)
                {
                    importExpCommand = new DelegateCommand(ImportExplanation, CanImportExplanation);
                }
                return importExpCommand;
            }
        }

        public ICommand DeleteSimilarCommand
        {
            get
            {
                if (deleteSimilarCommand == null)
                {
                    deleteSimilarCommand = new DelegateCommand(DeleteSimilar, CanDeleteSimilar);
                }
                return deleteSimilarCommand;
            }
        }

        public ICommand UpSimilarCommand
        {
            get
            {
                if (upSimilarCommand == null)
                {
                    upSimilarCommand = new DelegateCommand(UpSimilar, CanUpSimilar);
                }
                return upSimilarCommand;
            }
        }

        public ICommand DownSimilarCommand
        {
            get
            {
                if (downSimilarCommand == null)
                {
                    downSimilarCommand = new DelegateCommand(DownSimilar, CanDownSimilar);
                }
                return downSimilarCommand;
            }
        }

        public ICommand ExportSimilarCommand
        {
            get
            {
                if (exportSimilarCommand == null)
                {
                    exportSimilarCommand = new DelegateCommand(ExportSimilar, CanExportSimilar);
                }
                return exportSimilarCommand;
            }
        }

        public ICommand ImportSimilarCommand
        {
            get
            {
                if (importSimilarCommand == null)
                {
                    importSimilarCommand = new DelegateCommand(ImportSimilar, CanImportSimilar);
                }
                return importSimilarCommand;
            }
        }

        #endregion

        #region Public Methods

        public void SnippetTaken(Snippet snippet)
        {
            if (snippet.ChapterId != this.Chapter.Id || snippet.Page != this.CurrentPage)
            {
                UIHelper.MessageBox("The snippet belong to different Chapter/Page.");
                return;
            }

            snippet.Order = this.Snippets.Count;
            this.snippetRepository.AddSnippet(snippet);
            this.LoadSnippets();
            this.Snippet = this.Snippets[this.Snippets.Count - 1];
        }

        public void SimilarSnippetTaken(Snippet snippet)
        {
            snippet.Order = this.SimilarSnippets.Count;
            snippet.ParentId = this.Snippet.Id;
            this.snippetRepository.AddSnippet(snippet);
            this.LoadSimilarSnippets();
            this.SimilarSnippet = this.SimilarSnippets[this.SimilarSnippets.Count - 1];
        }

        public void SaveExplanation()
        {
            if (this.suspendEvents || this.Explanation == null)
            {
                return;
            }

            this.explanationRepository.Update(this.Explanation);
            this.DrawPage();
        }

        public void SaveSimilarSnippet()
        {
            if (this.suspendEvents || this.SimilarSnippet == null)
            {
                return;
            }

            this.snippetRepository.UpdateTop(this.SimilarSnippet.Id, this.SimilarSnippet.Top);
            this.DrawPage();
        }

        #endregion

        #region Command Methods

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
            newExplanation.Order = this.Explanations.Count;
            this.explanationRepository.AddExplanation(newExplanation);
            this.LoadExplanations();
            this.Explanation = this.Explanations[this.Explanations.Count - 1];
        }

        private bool CanDeleteExplanation()
        {
            return this.Explanation != null;
        }

        private void DeleteExplanation()
        {
            this.suspendEvents = true;
            int index = this.Explanations.IndexOf(this.Explanation);
            this.explanationRepository.Delete(this.Explanation.Id);
            this.LoadExplanations();
            this.Explanation = this.Explanations.Count > index ? this.Explanations[index] : null;
            this.suspendEvents = false;
        }

        private bool CanUpExplanation()
        {
            return this.Explanation != null && this.explanation != this.Explanations[0];
        }

        private void UpExplanation()
        {
            this.suspendEvents = true;
            int index = this.Explanations.IndexOf(this.Explanation);
            this.explanationRepository.Swap(this.Explanation, this.Explanations[index - 1]);
            this.LoadExplanations();
            this.Explanation = this.Explanations[index - 1];
            this.suspendEvents = false;
        }

        private bool CanDownExplanation()
        {
            return this.Explanation != null && this.explanation != this.Explanations[this.Explanations.Count - 1];
        }

        private void DownExplanation()
        {
            this.suspendEvents = true;
            int index = this.Explanations.IndexOf(this.Explanation);
            this.explanationRepository.Swap(this.Explanation, this.Explanations[index + 1]);
            this.LoadExplanations();
            this.Explanation = this.Explanations[index + 1];
            this.suspendEvents = false;
        }

        private bool CanImportExplanation()
        {
            return this.Snippet != null;
        }

        private void ImportExplanation()
        {
            var json = UIHelper.OpenFile();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var newExplanations = new JavaScriptSerializer().Deserialize<IList<Explanation>>(json);
                    int order = this.Explanations.Count;
                    foreach (var newExp in newExplanations)
                    {
                        var newExplanation = new Explanation();
                        newExplanation.SnippetId = this.Snippet.Id;
                        newExplanation.Type = newExp.Type;
                        newExplanation.Top = newExp.Top;
                        newExplanation.Text = newExp.Text;
                        newExplanation.Order = order;
                        order++;
                        this.explanationRepository.AddExplanation(newExplanation);
                    }

                    this.LoadExplanations();
                }
                catch (Exception ex)
                {
                    UIHelper.MessageBox("Can't Import file: " + ex.Message);
                }
            }
        }

        private bool CanExportExplanation()
        {
            return this.Snippet != null && this.Explanations.Count > 0;
        }

        private void ExportExplanation()
        {
            var json = new JavaScriptSerializer().Serialize(this.Explanations);
            UIHelper.SaveToFile(json);
        }

        private bool CanDeleteSimilar()
        {
            return this.SimilarSnippet != null;
        }

        private void DeleteSimilar()
        {
            this.suspendEvents = true;
            int index = this.SimilarSnippets.IndexOf(this.SimilarSnippet);
            this.snippetRepository.Delete(this.SimilarSnippet.Id);
            this.LoadSimilarSnippets();
            this.SimilarSnippet = this.SimilarSnippets.Count > index ? this.SimilarSnippets[index] : null;
            this.suspendEvents = false;
        }

        private bool CanUpSimilar()
        {
            return this.SimilarSnippet != null && this.SimilarSnippet != this.SimilarSnippets[0];
        }

        private void UpSimilar()
        {
            this.suspendEvents = true;
            int index = this.SimilarSnippets.IndexOf(this.SimilarSnippet);
            this.snippetRepository.Swap(this.SimilarSnippet, this.SimilarSnippets[index - 1]);
            this.LoadSimilarSnippets();
            this.SimilarSnippet = this.SimilarSnippets[index - 1];
            this.suspendEvents = false;
        }

        private bool CanDownSimilar()
        {
            return this.SimilarSnippet != null && this.SimilarSnippet != this.SimilarSnippets[this.SimilarSnippets.Count - 1];
        }

        private void DownSimilar()
        {
            this.suspendEvents = true;
            int index = this.SimilarSnippets.IndexOf(this.SimilarSnippet);
            this.snippetRepository.Swap(this.SimilarSnippet, this.SimilarSnippets[index + 1]);
            this.LoadSimilarSnippets();
            this.SimilarSnippet = this.SimilarSnippets[index + 1];
            this.suspendEvents = false;
        }

        private bool CanImportSimilar()
        {
            return this.Snippet != null;
        }

        private void ImportSimilar()
        {
            var json = UIHelper.OpenFile();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var newSimilars = new JavaScriptSerializer().Deserialize<IList<Snippet>>(json);
                    int order = this.SimilarSnippets.Count;
                    foreach (var newExp in newSimilars)
                    {
                        var newSnippet = new Snippet();
                        newSnippet.Order = newExp.Order;
                        newSnippet.ChapterId = newExp.ChapterId;
                        newSnippet.Page = newExp.Page;
                        newSnippet.StartVerse = newExp.StartVerse;
                        newSnippet.EndVerse = newExp.EndVerse;
                        newSnippet.StartLine = newExp.StartLine;
                        newSnippet.EndLine = newExp.EndLine;
                        newSnippet.StartPoint = newExp.StartPoint;
                        newSnippet.EndPoint = newExp.EndPoint;
                        newSnippet.Text = newExp.Text;
                        newSnippet.Rtf = newExp.Rtf;
                        newSnippet.Top = newExp.Top;
                        newSnippet.ParentId = this.Snippet.Id;
                        order++;
                        this.snippetRepository.AddSnippet(newSnippet);
                    }

                    this.LoadSimilarSnippets();
                }
                catch (Exception ex)
                {
                    UIHelper.MessageBox("Can't Import file: " + ex.Message);
                }
            }
        }

        private bool CanExportSimilar()
        {
            return this.Snippet != null && this.SimilarSnippets.Count > 0;
        }

        private void ExportSimilar()
        {
            var json = new JavaScriptSerializer().Serialize(this.SimilarSnippets);
            UIHelper.SaveToFile(json);
        }

        #endregion

        #region Private Methods

        private void LoadSnippet()
        {
            this.HasSnippet = this.Snippet != null;
            if (this.HasSnippet)
            {
                var worker = new BackgroundWorker();
                worker.DoWork += worker_DoWork;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                this.HasSnippet = false;
                worker.RunWorkerAsync();
            }
        }

        private void LoadExplanations()
        {
            if (this.Snippet != null)
            {
                this.Explanations.Clear();
                var explanations = this.explanationRepository.GetExplanations(this.Snippet.Id);
                explanations.ForEach(S => this.Explanations.Add(S));
                this.DrawPage();
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Page = BitmapHelper.LoadPage(this.Snippet.Page);
            this.Page = BitmapHelper.FocusSelection(this.Page, this.Snippet);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.LoadExplanations();
            this.LoadSimilarSnippets();
            this.HasSnippet = true;
        }

        private void LoadExplanation()
        {
            this.HasExplanation = this.Explanation != null;
            if (this.HasExplanation)
            {
                this.suspendEvents = true;
                this.ExplanationType = (int)this.Explanation.Type;
                this.ExplanationTop = this.Explanation.Top;
                this.ExplanationText = this.Explanation.Text;
                this.HasIcon = this.explanation.Type != Models.ExplanationType.Explain;
                if (this.HasIcon)
                {
                    this.Icons.Clear();
                    if (this.explanation.Type == Models.ExplanationType.Note)
                    {
                        Enum.GetNames(typeof(NoteIcons)).ToList().ForEach(T => this.Icons.Add(T));
                    }
                    else
                    {
                        Enum.GetNames(typeof(GuideIcons)).ToList().ForEach(T => this.Icons.Add(T));
                    }
                    this.IconIndex = this.Explanation.Icon;
                }
                this.suspendEvents = false;
            }
        }

        private void DrawPage()
        {
            var expPage = BitmapHelper.DrawExplanation((Bitmap)this.Page.Clone(), this.Explanations);
            expPage = BitmapHelper.DrawSimilarSnippets(expPage, this.SimilarSnippets);
            expPage.Save(@"E:\Fun\Tafseer\Backup\Images\Debug\1.png", System.Drawing.Imaging.ImageFormat.Png);
            this.ImageSource = BitmapHelper.BitmapToImageSource(expPage);
        }

        private void LoadSnippets()
        {
            this.Snippets.Clear();
            var snippets = this.snippetRepository.GetSnippets(this.chapter.Id, this.CurrentPage);
            snippets.ForEach(S => this.Snippets.Add(S));
        }

        private void LoadSimilarSnippet()
        {
            if (this.SimilarSnippet != null)
            {
                this.SimilarTop = this.SimilarSnippet.Top;
            }
        }

        private void LoadSimilarSnippets()
        {
            this.SimilarSnippets.Clear();
            var similars = this.snippetRepository.GetSnippetsByParentId(this.Snippet.Id);
            similars.ForEach(S => this.SimilarSnippets.Add(S));
            this.DrawPage();
        }

        #endregion
    }
}