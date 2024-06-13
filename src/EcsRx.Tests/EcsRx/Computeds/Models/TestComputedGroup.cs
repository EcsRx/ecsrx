using System;
using EcsRx.Computeds.Groups;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.Tests.Models;
using R3;

namespace EcsRx.Tests.EcsRx.Computeds.Models
{
    public class TestComputedGroup : ComputedGroup
    {
        public Subject<bool> ManuallyRefresh = new Subject<bool>();
        
        public TestComputedGroup(IObservableGroup internalObservableGroup) : base(internalObservableGroup)
        { }

        public override Observable<bool> RefreshWhen()
        { return ManuallyRefresh; }
        
        public override bool IsEntityApplicable(IEntity entity)
        { return entity.HasComponent<TestComponentOne>(); }
    }
}