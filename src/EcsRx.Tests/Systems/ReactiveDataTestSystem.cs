using System;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;
using R3;

namespace EcsRx.Tests.Systems
{
    public class ReactiveDataTestSystem : IReactToDataSystem<float>
    {
        public IGroup Group => new Group().WithComponent<TestComponentOne>();

        public Observable<float> ReactToData(IEntity entity)
        {
            return Observable.Timer(TimeSpan.FromSeconds(1)).Select(x => 0.1f);
        }

        public void Process(IEntity entity, float reactionData)
        {
            throw new NotImplementedException();
        }
    }
}