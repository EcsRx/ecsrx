using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SystemsRx.Attributes;
using SystemsRx.Systems.Conventional;
using SystemsRx.Types;
using EcsRx.Collections;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Components;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Events;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Pipelines;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EcsRx.Examples.ExampleApps.DataPipelinesExample.Systems
{
    [Priority(PriorityTypes.SuperLow)]
    public class TriggerPipelineSystem : IReactToEventSystem<SavePipelineEvent>
    {
        public PostJsonHttpPipeline SaveJsonPipeline { get; }
        public IObservableGroup PlayerGroup { get; }
        
        public TriggerPipelineSystem(PostJsonHttpPipeline saveJsonPipeline, IObservableGroupManager observableGroupManager)
        {
            SaveJsonPipeline = saveJsonPipeline;
            PlayerGroup = GetPlayerGroup(observableGroupManager);
        }

        public async Task TriggerPipeline(PlayerStateComponent playerState)
        {
            var httpResponse = (HttpResponseMessage) await SaveJsonPipeline.Execute(playerState);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            var prettyResponse = MakeDataPretty(responseContent);
            Console.WriteLine($"Server Responded With {prettyResponse}");
        }

        // Feel free to output everything in the JToken if you want, only showing data for simplicity
        public string MakeDataPretty(string jsonData)
        { return JToken.Parse(jsonData)["data"].ToString(Formatting.Indented); }

        public IObservableGroup GetPlayerGroup(IObservableGroupManager observableGroupManager)
        { return observableGroupManager.GetObservableGroup(new Group(typeof(PlayerStateComponent))); }
        
        public void Process(SavePipelineEvent eventData)
        {
            var playerState = PlayerGroup.Single().GetComponent<PlayerStateComponent>();
            Task.Run(() => TriggerPipeline(playerState));
        }
    }
}