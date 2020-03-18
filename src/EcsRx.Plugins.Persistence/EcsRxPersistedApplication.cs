using System.IO;
using System.Threading.Tasks;
using EcsRx.Collections.Database;
using EcsRx.Infrastructure;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Plugins.Persistence.Modules;
using EcsRx.Plugins.Persistence.Pipelines;

namespace EcsRx.Plugins.Persistence
{
    public abstract class EcsRxPersistedApplication : EcsRxApplication
    {
        public ISaveEntityDatabasePipeline SaveEntityDatabasePipeline;
        public ILoadEntityDatabasePipeline LoadEntityDatabasePipeline;

        public virtual string EntityDatabaseFile => PersistityModule.DefaultEntityDatabaseFile;
        
        protected override void LoadPlugins()
        {
            base.LoadPlugins();
            RegisterPlugin(new PersistencePlugin());
        }
        
        protected override void ResolveApplicationDependencies()
        {
            SaveEntityDatabasePipeline = Container.Resolve<ISaveEntityDatabasePipeline>();
            LoadEntityDatabasePipeline = Container.Resolve<ILoadEntityDatabasePipeline>();

            LoadEntityDatabase().Wait();
            base.ResolveApplicationDependencies();
        }
        
        protected virtual async Task LoadEntityDatabase()
        {
            // If there is no file just ignore loading
            if (!File.Exists(EntityDatabaseFile)) { return; }
            
            var entityDatabase = await LoadEntityDatabasePipeline.Execute<EntityDatabase>(null);
            Container.Unbind<IEntityDatabase>();
            Container.Bind<IEntityDatabase>(x => x.ToInstance(entityDatabase));
        }
        
        protected virtual Task SaveEntityDatabase()
        {
            // Update our database with any changes that have happened since it loaded
            return SaveEntityDatabasePipeline.Execute(EntityCollectionManager.EntityDatabase, null);
        }
    }
}