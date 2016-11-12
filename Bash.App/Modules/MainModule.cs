using Bash.Common.Data;
using Bash.App.ViewModels;
using Ninject.Modules;
using Bash.App.Data;

namespace Bash.App.Modules
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IMainViewModel>().To<MainViewModel>().InSingletonScope();
            this.Bind<ICategoryViewModel>().To<CategoryViewModel>().InSingletonScope();
            this.Bind<ICommentsViewModel>().To<CommentsViewModel>().InSingletonScope();

            this.Bind<IFullyCachedBashClient>().To<FullyCachedBashClient>().InSingletonScope();
            this.Bind<ICachedBashClient>().To<CachedBashClient>().InSingletonScope();
            this.Bind<IBashClient>().To<BashClient>().InSingletonScope();

            this.Bind<IFavoriteManager>().To<FavoriteManager>().InSingletonScope();
        }
    }
}
