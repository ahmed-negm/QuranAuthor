namespace QuranAuthor.ViewModels
{
    public class TafseerViewModel : ViewModelBase
    {
        #region Private Members

        /*
         private DelegateCommand computeHashCommand;
         */

        #endregion

        #region Constructor

        public TafseerViewModel()
        {
            
        }

        #endregion

        #region Public Properties

        /*
        public string ComputedHash
        {
            get { return computedHash; }
            set
            {
                computedHash = value;
                OnPropertyChanged("ComputedHash");
            }
        }
        */

        #endregion

        #region Commands

        /*
        public ICommand ComputeHashCommand
        {
            get
            {
                if (computeHashCommand == null)
                {
                    computeHashCommand = new DelegateCommand(ComputeHash, CanComputeHash);
                }
                return computeHashCommand;
            }
        }

        private bool CanComputeHash()
        {
            if (!string.IsNullOrEmpty(this.PlainText) && !string.IsNullOrEmpty(this.Salt))
                return true;
            else
                return false;
        }

        private void ComputeHash()
        {
            hashing.ComputingResult();
            ComputedHash = Result;
        }
        */
        
        #endregion
    }
}
