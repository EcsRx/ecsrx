using System;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.MicroRx.Subjects;
using EcsRx.Plugins.Computeds.Groups;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Plugins.Computeds.Models
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