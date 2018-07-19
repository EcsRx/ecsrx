using System;
using System.Linq;
using System.Reactive.Subjects;
using EcsRx.Computed;
using EcsRx.Groups.Observable;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.ComputedGroups
{
    public class TestComputedGroupData : ComputedGroupData<double>
    {
        public Subject<bool> ManuallyRefresh = new Subject<bool>();
        
        public TestComputedGroupData(IObservableGroup internalObservableGroup) : base(internalObservableGroup)
        {}

        public override IObservable<bool> RefreshWhen()
        { return ManuallyRefresh; }

        public override double Filter(IObservableGroup observableGroup)
        { return observableGroup.Where(x => x.HasComponent<TestComponentThree>()).Average(x => x.GetHashCode()); }
    }
}