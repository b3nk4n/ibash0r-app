using Bash.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bash.Common.Data
{
    public interface IFavoriteManager
    {
        void AddToFavorites(BashData bashData);

        void RemoveFromFavorites(BashData bashData);

        BashCollection GetData();

        void SaveData();

        bool IsFavorite(BashData bashData);

        bool HasDataChanged { get; }
    }
}
