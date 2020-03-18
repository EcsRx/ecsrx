using System;
using System.IO;
using System.Threading.Tasks;
using EcsRx.Collections.Database;
using EcsRx.Entities;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Components;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Events;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Modules;
using EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Blueprints;
using EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Modules;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Plugins.Persistence.Pipelines;
using Persistity.Pipelines;

namespace EcsRx.Examples.ExampleApps.LoadingEntityDatabase
{
    public class LoadingEntityDatabaseApplication : EcsRxConsoleApplication
    {
        private bool _quit;

        private ISaveEntityDatabasePipeline _saveEntityDatabasePipeline;
        private ILoadEntityDatabasePipeline _loadEntityDatabasePipeline;

        protected override void LoadModules()
        {
            base.LoadModules();
            
            // Add support for serializing/deserializing System.Numerics
            Container.LoadModule<EnableNumericsModule>();
        }
        
        protected override void ResolveApplicationDependencies()
        {
            // Replace our default binary entity database with json,
            // we do this here as the plugins loaded by now and we need to override 
            // bindings set in place by the plugin.
            Container.LoadModule<JsonEntityDatabaseModule>();
            
            _saveEntityDatabasePipeline = Container.Resolve<ISaveEntityDatabasePipeline>();
            _loadEntityDatabasePipeline = Container.Resolve<ILoadEntityDatabasePipeline>();

            LoadEntityDatabase().Wait();
            base.ResolveApplicationDependencies();
        }

        private async Task LoadEntityDatabase()
        {
            // If we have an existing entity database load it
            if (!File.Exists(JsonEntityDatabaseModule.CustomEntityDatabaseFile)) { return; }
            
            var entityDatabase = await _loadEntityDatabasePipeline.Execute<EntityDatabase>(null);
            Container.Unbind<IEntityDatabase>();
            Container.Bind<IEntityDatabase>(x => x.ToInstance(entityDatabase));
        }
        
        private void SaveEntityDatabase()
        {
            // Update our database with any changes that have happened since it loaded
            _saveEntityDatabasePipeline
                .Execute(EntityCollectionManager.EntityDatabase, null)
                .Wait();
        }

        protected override void ApplicationStarted()
        {
            HandleInput();
        }
        
        private void HandleInput()
        {
            var defaultCollection = EntityCollectionManager.EntityDatabase.GetCollection();
            var randomBlueprint = new RandomEntityBlueprint();

            while (!_quit)
            {
                Console.Clear();
                Console.WriteLine("Debug this application and look at whats inside the entity database for more info");
                Console.WriteLine("When the application is closed it will save the current entity database");
                Console.WriteLine("Look in the bin folder for an entity-database.json file, alter it if you want");
                Console.WriteLine();
                Console.WriteLine($" - {defaultCollection.Count} Entities Loaded");
                Console.WriteLine();
                Console.WriteLine(" - Press Enter To Add Another Random Entity");
                Console.WriteLine(" - Press Escape To Save and Quit");
                
                var keyPressed = Console.ReadKey();
                if (keyPressed.Key == ConsoleKey.Enter)
                { defaultCollection.CreateEntity(randomBlueprint); }
                else if (keyPressed.Key == ConsoleKey.Escape)
                { _quit = true; }
            }

            SaveEntityDatabase();
        }
    }
}