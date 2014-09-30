using Bash.App.Data;
using Bash.App.ViewModels;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.Modules
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ISearchViewModel>().To<SearchViewModel>().InSingletonScope();
            this.Bind<ICategoryViewModel>().To<CategoryViewModel>().InSingletonScope();

            this.Bind<IBashClient>().To<BashClient>().InSingletonScope();
        }
    }
}
