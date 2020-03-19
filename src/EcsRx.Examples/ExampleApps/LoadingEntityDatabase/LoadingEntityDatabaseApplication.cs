using System;
using EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Blueprints;
using EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Modules;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Infrastructure.Ninject;
using EcsRx.Plugins.Persistence;

namespace EcsRx.Examples.ExampleApps.LoadingEntityDatabase
{
    // We extend from EcsRxPersistedApplication which has built in helpers for persisting entity DB
    public class LoadingEntityDatabaseApplication : EcsRxPersistedApplication
    {
        public override IDependencyContainer Container { get; }  = new NinjectDependencyContainer();

        // Tell it to look for the JSON file now rather than the binary one
        public override string EntityDatabaseFile => JsonEntityDatabaseModule.CustomEntityDatabaseFile;
        
        private bool _quit;

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
            
            base.ResolveApplicationDependencies();
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