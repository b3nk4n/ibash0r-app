using Bash.App.Data;
using Bash.App.Resources;
using PhoneKit.Framework.Controls;
using PhoneKit.Framework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ninject;

namespace Bash.App.Controls
{
    public class BackupControlViewModel : BackupControlViewModelBase
    {
        private IFavoriteManager _favoriteManager;

        public BackupControlViewModel()
            : base("0000000044119663", AppResources.ApplicationTitle) // TODO: onedrive id !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        {
            _favoriteManager = App.Injector.Get<IFavoriteManager>();
        }

        protected override IDictionary<string, IList<string>> GetBackupDirectoriesAndFiles()
        {
            var pathsAndFiles = new Dictionary<string, IList<string>>();

            // favorites
            var favList = new List<string>();
            var favoriteDataToBackup = _favoriteManager.GetData();
            if (favoriteDataToBackup.Contents.Data.Count > 0)
            {
                favList.Add(FavoriteManager.FAV_DATA_FILE);
            }
            pathsAndFiles.Add("/", favList);
            return pathsAndFiles;
        }

        protected override IEnumerable<string> GetScopes()
        {
            return OneDriveManager.SCOPES_DEFAULT;
        }

        protected override void BeforeBackup(string backupName)
        {
            base.BeforeBackup(backupName);

            _favoriteManager.SaveData();
        }

        protected override void AfterBackup(string backupName, bool success)
        {
            base.AfterBackup(backupName, success);

            if (success)
            {
                MessageBox.Show(string.Format(AppResources.MessageBoxBackupSuccessText, backupName), AppResources.MessageBoxInfoTitle, MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(string.Format(AppResources.MessageBoxBackupErrorText, backupName), AppResources.MessageBoxWarningTitle, MessageBoxButton.OK);
            }
        }

        protected override void AfterRestore(string backupName, bool success)
        {
            base.AfterRestore(backupName, success);

            if (success)
            {
                // load new data to memory
                _favoriteManager.GetData(); // TODO: does this work???

                MessageBox.Show(string.Format(AppResources.MessageBoxRestoreSuccessText, backupName), AppResources.MessageBoxInfoTitle, MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(string.Format(AppResources.MessageBoxRestoreErrorText, backupName), AppResources.MessageBoxWarningTitle, MessageBoxButton.OK);
            }
        }
    }
}
