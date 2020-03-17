using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EcsRx.Events;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Components;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Custom;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Persistity.Pipelines;

namespace EcsRx.Examples.ExampleApps.DataPipelinesExample.Systems
{
    public class TriggerPipelineSystem : EventReactionSystem<SavePipelineEvent>
    {
        public override IGroup Group => new Group(typeof(PlayerStateComponent));
        
        public ISendDataPipeline SaveJsonPipeline { get; }

        public TriggerPipelineSystem(IEventSystem eventSystem, ISendDataPipeline saveJsonPipeline) : base(eventSystem)
        {
            SaveJsonPipeline = saveJsonPipeline;
        }
        
        public override void EventTriggered(SavePipelineEvent eventData)
        {
            var entity = ObservableGroup.Single();
            var playerState = entity.GetComponent<PlayerStateComponent>();
            Task.Run(() => TriggerPipeline(playerState));
        }

        public async Task TriggerPipeline(PlayerStateComponent playerState)
        {
            var httpResponse = (HttpResponseMessage) await SaveJsonPipeline.Execute(playerState, null);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            var prettyResponse = MakeDataPretty(responseContent);
            Console.WriteLine($"Server Responded With {prettyResponse}");
        }

        // Feel free to output everything in the JToken if you want, only showing data for simplicity
        public string MakeDataPretty(string jsonData)
        { return JToken.Parse(jsonData)["data"].ToString(Formatting.Indented); }
    }
}