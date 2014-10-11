using Bash.App.Resources;
using PhoneKit.Framework.Controls;
using PhoneKit.Framework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bash.App.Controls
{
    class BackupControlViewModel : BackupControlViewModelBase
    {
        public BackupControlViewModel()
            : base("0000000044119663", AppResources.ApplicationTitle) // TODO: onedrive id !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        {

        }

        protected override IDictionary<string, IList<string>> GetBackupDirectoriesAndFiles()
        {
            var pathsAndFiles = new Dictionary<string, IList<string>>();

            // note and archive
            //var naList = new List<string>();
            //if (NapStatisticsViewModel.Instance.NapList.Count > 0)
            //{
            //    naList.Add("statistics.data");
            //}
            //pathsAndFiles.Add("/", naList);
            return pathsAndFiles;
        }

        protected override IEnumerable<string> GetScopes()
        {
            return OneDriveManager.SCOPES_DEFAULT;
        }

        protected override void BeforeBackup(string backupName)
        {
            base.BeforeBackup(backupName);

            //NapStatisticsViewModel.Instance.Save();
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
                //NapStatisticsViewModel.Instance.Load(true);

                MessageBox.Show(string.Format(AppResources.MessageBoxRestoreSuccessText, backupName), AppResources.MessageBoxInfoTitle, MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(string.Format(AppResources.MessageBoxRestoreErrorText, backupName), AppResources.MessageBoxWarningTitle, MessageBoxButton.OK);
            }
        }
    }
}
