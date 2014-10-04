﻿using Bash.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Bash.App.ViewModels
{
    interface ICategoryViewModel
    {
        Task<bool> LoadQuotesAsync(string order);

        bool LoadFavorites();

        Task<bool> SearchQuotesAsync(string term);

        BashData CurrentBashData { get; }

        NavigationService NavigationService { set; }

        bool IsCurrentBashFavorite { get; }

        ICommand NextCommand { get; }

        ICommand PreviousCommand { get; }

        ICommand RatePositiveCommand { get; }

        ICommand RateNegativeCommand { get; }

        ICommand ShowCommentsCommand { get; }

        ICommand AddToFavoritesCommand { get; }

        ICommand RemoveFromFavoritesCommand { get; }
    }
}
