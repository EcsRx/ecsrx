using System;
using System.Reactive.Linq;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Components;

namespace EcsRx.Tests.Systems
{
    public class ReactiveDataTestSystem : IReactToDataSystem<float>
    {
        public IGroup TargetGroup => new Group().WithComponent<TestComponentOne>();

        public IObservable<float> ReactToData(IEntity entity)
        {
            return Observable.Timer(TimeSpan.FromSeconds(1)).Select(x => 0.1f);
        }

        public void Execute(IEntity entity, float reactionData)
        {
            throw new System.NotImplementedException();
        }
    }
}