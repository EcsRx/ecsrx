using System;
using System.Threading.Tasks;
using SystemsRx.Infrastructure.Dependencies;
using EcsRx.Plugins.Persistence.Extensions;
using EcsRx.Plugins.Persistence.Pipelines;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Persistity.Core;

namespace EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Modules
{
    public class EntityDebugModule : IDependencyModule
    {
        public const string DebugPipeline = "DebugPipeline";
        
        public void Setup(IDependencyContainer container)
        {
            // Make a new pipeline for showing json output
            container.BuildPipeline(DebugPipeline, builder => builder
                // Fork from the 2nd step (serializer) on the existing JSON Saving Pipeline
                .ForkDataFrom<ISaveEntityDatabasePipeline>(2)
                // Then spit out the json to the console in a nice way
                .ThenInvoke(WriteEntityData));
        }

        private Task<DataObject> WriteEntityData(DataObject data)
        {
            var prettyText = JToken.Parse(data.AsString).ToString(Formatting.Indented);
            Console.WriteLine(prettyText);
            return Task.FromResult(data);
        }
    }
}