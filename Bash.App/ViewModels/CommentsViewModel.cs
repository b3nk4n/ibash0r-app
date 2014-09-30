using Bash.App.Data;
using Bash.App.Models;
using PhoneKit.Framework.Core.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.ViewModels
{
    class CommentsViewModel : ViewModelBase, ICommentsViewModel
    {
        #region Members

        private IBashClient _bashClient;

        private BashComments _bashComments;

        #endregion

        #region Constructors
        public CommentsViewModel(IBashClient bashClient)
        {
            _bashClient = bashClient;
        }

        #endregion

        #region Public Methods

        public async Task<bool> LoadCommentsAsync(string id)
        {
            var result = await _bashClient.GetCommentsAsync(id);

            if (result == null)
                return false;

            BashComments = result;
            return true;
        }

        #endregion

        #region Private Methods

        #endregion

        #region Properties

        public BashComments BashComments
        {
            get { return _bashComments; }
            set
            {
                if (_bashComments != value)
                {
                    _bashComments = value;
                    NotifyPropertyChanged("BashComments");
                }
            }
        }

        #endregion
    }
}
