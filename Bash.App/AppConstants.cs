using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App
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

        public const string ORDER_VALUE_NEW = "new";
        public const string ORDER_VALUE_BEST = "best";
        public const string ORDER_VALUE_RANDOM = "random";

        public const string TYPE_VALUE_POS = "pos";
        public const string TYPE_VALUE_NEG = "neg";
    }
}
