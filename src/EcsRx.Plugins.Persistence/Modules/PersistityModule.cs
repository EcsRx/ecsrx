using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Plugins.Persistence.Transformers;

namespace EcsRx.Plugins.Persistence.Modules
{
    public class PersistityModule : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            container.Bind<IEntityDataTransformer, EntityDataTransformer>();
            container.Bind<IEntityCollectionDataTransformer, EntityCollectionDataTransformer>();
        }
    }
}