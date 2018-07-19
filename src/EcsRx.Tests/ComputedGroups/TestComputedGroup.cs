using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using EcsRx.Entities;
using EcsRx.Groups.Computed;
using EcsRx.Groups.Observable;
using EcsRx.Polyfills;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.ComputedGroups
{
    public class TestComputedGroup : ComputedGroup
    {
        public Subject<bool> ManuallyRefresh = new Subject<bool>();
        
        public TestComputedGroup(IObservableGroup internalObservableGroup) : base(internalObservableGroup)
        { }

        public override IObservable<bool> RefreshWhen()
        { return ManuallyRefresh; }
        
        public override bool IsEntityApplicable(IEntity entity)
        { return entity.HasComponent<TestComponentOne>(); }
    }
}