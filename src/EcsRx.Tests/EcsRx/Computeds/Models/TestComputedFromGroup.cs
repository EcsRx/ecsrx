using System;
using System.Linq;
using EcsRx.Computeds;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.Tests.Models;
using SystemsRx.MicroRx.Subjects;

namespace EcsRx.Tests.EcsRx.Computeds.Models
{
    public class TestComputedFromGroup : ComputedFromGroup<double>
    {
        public Subject<bool> ManuallyRefresh = new Subject<bool>();
        
        public TestComputedFromGroup(IObservableGroup internalObservableGroup) : base(internalObservableGroup)
        {}

        public override IObservable<bool> RefreshWhen()
        { return ManuallyRefresh; }

        public override double Transform(IObservableGroup observableGroup)
        { return observableGroup.Where(x => x.HasComponent<TestComponentThree>()).Average(x => x.GetHashCode()); }
    }
}