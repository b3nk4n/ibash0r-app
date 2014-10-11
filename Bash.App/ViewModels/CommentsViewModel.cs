﻿using Bash.App.Data;
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

        private bool _isBusy;

        #endregion

        #region Constructors
        public CommentsViewModel(ICachedBashClient bashClient)
        {
            _bashClient = bashClient;
        }

        #endregion

        #region Public Methods

        public async Task<bool> LoadCommentsAsync(int id)
        {
            IsBusy = true;
            var result = await _bashClient.GetCommentsAsync(id);

            if (result == null)
            {
                IsBusy = false;
                return false;
            }
                

            BashComments = result;
            IsBusy = false;
            return true;
        }

        public void Reset()
        {
            BashComments = null;
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

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    NotifyPropertyChanged("IsBusy");
                }
            }
        }

        #endregion
    }
}
