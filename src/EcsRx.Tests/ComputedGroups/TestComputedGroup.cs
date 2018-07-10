using System;
using System.Reactive.Linq;
using EcsRx.Entities;
using EcsRx.Groups.Computed;
using EcsRx.Groups.Observable;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.ComputedGroups
{
    public class TestComputedGroup : ComputedGroup
    {
        public TestComputedGroup(IObservableGroup internalObservableGroup) : base(internalObservableGroup)
        {}

        public override IObservable<bool> RefreshWhen()
        { return Observable.Empty<bool>(); }
        
        public override bool IsEntityApplicable(IEntity entity)
        { return entity.HasComponent<TestComponentOne>(); }
    }
}