using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using SystemsRx.MicroRx.Subjects;
using EcsRx.Plugins.Computeds.Collections;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Plugins.Computeds.Models
{
    public class TestComputedCollectionFromGroup : ComputedCollectionFromGroup<int>
    {
        public Subject<bool> ManuallyRefresh = new Subject<bool>();
        
        public TestComputedCollectionFromGroup(IObservableGroup internalObservableGroup) : base(internalObservableGroup)
        { }

        public override IObservable<bool> RefreshWhen()
        { return ManuallyRefresh; }

        public override bool ShouldTransform(IEntity entity)
        { return entity.HasComponent<TestComponentThree>(); }

        public override int Transform(IEntity entity)
        { return entity.GetHashCode(); }

        public override IEnumerable<int> PostProcess(IEnumerable<int> data)
        { return data.OrderBy(x => x); }
    }
}