using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Computeds.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.Tests.Models;
using R3;

namespace EcsRx.Tests.EcsRx.Computeds.Models
{
    public class TestComputedCollectionFromGroup : ComputedCollectionFromGroup<int>
    {
        public Subject<bool> ManuallyRefresh = new Subject<bool>();
        
        public TestComputedCollectionFromGroup(IObservableGroup internalObservableGroup) : base(internalObservableGroup)
        { }

        public override Observable<bool> RefreshWhen()
        { return ManuallyRefresh; }

        public override bool ShouldTransform(IEntity entity)
        { return entity.HasComponent<TestComponentThree>(); }

        public override int Transform(IEntity entity)
        { return entity.GetHashCode(); }

        public override IEnumerable<int> PostProcess(IEnumerable<int> data)
        { return data.OrderBy(x => x); }
    }
}