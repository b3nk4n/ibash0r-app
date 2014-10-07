using Bash.App.Models;
using PhoneKit.Framework.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.Data
{
    public class FavoriteManager : IFavoriteManager
    {
        #region Members

        private const string FAV_DATA_FILE = "bash_fav.data";

        private BashCollection _favData;
        private bool _hasDataChanged;

        private List<string> _markToRemoveList = new List<string>();

        #endregion

        #region Constructors

        #endregion

        #region Public Methods

        public void AddToFavorites(BashData bashData)
        {
            var dataList = GetData();

            // make sure the new item is not marked to remove
            _markToRemoveList.Remove(bashData.Id);

            // add to favorites list when not already on it
            if (!dataList.Contents.Data.Contains(bashData))
                dataList.Contents.Data.Add(bashData);

            HasDataChanged = true;
        }

        public void RemoveFromFavorites(BashData bashData)
        {
            var dataList = GetData();

            if (!_markToRemoveList.Contains(bashData.Id))
                _markToRemoveList.Add(bashData.Id);

            HasDataChanged = true;
        }

        public BashCollection GetData()
        {
            if (_favData == null)
                _favData = LoadData();

            return _favData;
        }

        public void SaveData()
        {
            if (HasDataChanged && _favData != null)
            {
                // remove marked items
                foreach (var item in _markToRemoveList)
                {
                    _favData.Contents.Data.Find(i => i.Id == item);
                }
                _markToRemoveList.Clear();

                // save file
                StorageHelper.SaveAsSerializedFile<BashCollection>(FAV_DATA_FILE, _favData);
            }
        }

        public bool IsFavorite(BashData bashData)
        {
            if (_favData == null || bashData == null)
                return false;

            var result = _favData.Contents.Data.Contains(bashData);

            if (_markToRemoveList.Contains(bashData.Id))
            {
                result = false;
            }

            return result;
        }

        #endregion

        #region Private Methods

        private BashCollection LoadData()
        {
            if (StorageHelper.FileExists(FAV_DATA_FILE))
            {
                return StorageHelper.LoadSerializedFile<BashCollection>(FAV_DATA_FILE);
            }

            return new BashCollection();
        }

        #endregion

        #region Properties

        public bool HasDataChanged
        {
            get { return _hasDataChanged; }
            private set { _hasDataChanged = value; }
        }

        #endregion
    }
}
