using System;
using System.Reactive.Linq;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Systems
{
    public class ReactiveDataTestSystem : IReactToDataSystem<float>
    {
        public IGroup Group => new Group().WithComponent<TestComponentOne>();

        public IObservable<float> ReactToData(IEntity entity)
        {
            return Observable.Timer(TimeSpan.FromSeconds(1)).Select(x => 0.1f);
        }

        public void Process(IEntity entity, float reactionData)
        {
            throw new NotImplementedException();
        }
    }
}