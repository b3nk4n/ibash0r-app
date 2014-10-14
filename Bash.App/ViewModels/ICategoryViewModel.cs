using Bash.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Bash.App.ViewModels
{
    public enum CategoryState { None, New, Best, Search, Random, Favorites, Warte }

    interface ICategoryViewModel
    {
        Task<bool> LoadQuotesAsync(string order, bool forceReload = false);

        Task<bool> LoadWarteAsync(bool forceReload = false);

        bool LoadFavorites();

        Task<bool> SearchQuotesAsync(string term);


        void Reset();

        BashData CurrentBashData { get; }

        int CurrentBashDataIndex { get; set; }

        NavigationService NavigationService { set; }

        bool IsCurrentBashFavorite { get; }

        bool IsBusy { get; }

        int BashNumber { get; }

        int BashCount { get; }

        string JumpPageNumber { get; set; }

        bool IsDataFreshlyLoaded { get; }

        void SaveState();

        void RestoreState();

        bool ShowFavoritesInfo { get; }

        bool ShowSearchNoResultsInfo { get; }

        bool ShowLoadingFailedInfo { get; }

        CategoryState CategoryState { get; set; }

        ICommand NextCommand { get; }

        ICommand PreviousCommand { get; }

        ICommand RatePositiveCommand { get; }

        ICommand RateNegativeCommand { get; }

        ICommand ShowCommentsCommand { get; }

        ICommand AddToFavoritesCommand { get; }

        ICommand RemoveFromFavoritesCommand { get; }

        ICommand PlaceholderCommand { get; }

        ICommand ShareWhatsAppCommand { get; }

        ICommand ShareClipboardCommand { get; }

        ICommand ShareLinkCommand { get; }

        ICommand ShareContentCommand { get; }

        ICommand JumpToCommand { get; }

        ICommand RefreshCommand { get; }

        ICommand OpenInBrowserCommand { get; }
    }
}
