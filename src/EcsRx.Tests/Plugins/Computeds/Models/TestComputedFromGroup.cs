using System;
using System.Linq;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using SystemsRx.MicroRx.Subjects;
using EcsRx.Plugins.Computeds;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Plugins.Computeds.Models
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