using System.Windows.Media;

namespace Bash.Common
{
    /// <summary>
    /// Gloabal application constants
    /// </summary>
    public static class AppConstants
    {
        public const string PARAM_TERM = "term";
        public const string PARAM_TYPE = "type";
        public const string PARAM_ID = "id";
        public const string PARAM_ORDER = "order";
        public const string PARAM_NUMBER = "number";
        public const string PARAM_PAGE = "page";
        public const string PARAM_FAVORITES = "favorites";
        public const string PARAM_WARTE = "warte";

        public const string ORDER_VALUE_NEW = "new";
        public const string ORDER_VALUE_BEST = "best";
        public const string ORDER_VALUE_RANDOM = "random";

        public const string TYPE_VALUE_POS = "pos";
        public const string TYPE_VALUE_NEG = "neg";

        public const int QUOTES_COUNT = 999;
        public const int QUOTES_COUNT_PRELOAD = 50; // keep this value not too low, because when the user never visits the "random" page, other items will not be shown on the lock screen.

        public static readonly Color[] COLORS = { 
            Color.FromArgb(255,21,86,173),// #1556ad
            Color.FromArgb(255, 7,58,103),
            Color.FromArgb(255,15,74,143),
            Color.FromArgb(255,11,66,123),
            Color.FromArgb(255,19,82,163),
            Color.FromArgb(255, 9,62,113),
            Color.FromArgb(255,17,78,153),
            Color.FromArgb(255,13,70,133)
            };

        public static readonly Color SERVER_COLOR = Color.FromArgb(255, 229, 36, 22); // #e52416

        public const string IAP_AWESOME_EDITION = "awesome_edition";

        public const string BACKGROUND_TASK_NAME = "Bash.BackgroundTask";
        public const string BACKGROUND_TASK_DESC = "Aktualisiert deinen Sperrbildschirm in regelmäßigen Zeitabständen.";
    }
}
