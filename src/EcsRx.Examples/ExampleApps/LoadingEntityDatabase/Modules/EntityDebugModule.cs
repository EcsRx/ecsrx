using System;
using System.Threading.Tasks;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Plugins.Persistence.Extensions;
using EcsRx.Plugins.Persistence.Pipelines;
using LazyData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Modules
{
    public class EntityDebugModule : IDependencyModule
    {
        public const string DebugPipeline = "DebugPipeline";
        
        public void Setup(IDependencyContainer container)
        {
            container.BuildPipeline(DebugPipeline, builder => builder
                .ForkDataFrom<ISaveEntityDatabasePipeline>(2)
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