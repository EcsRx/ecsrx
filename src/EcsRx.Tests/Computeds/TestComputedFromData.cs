using System;
using System.Linq;
using System.Reactive.Subjects;
using EcsRx.Computed;
using EcsRx.Entities;
using EcsRx.Groups.Observable;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.ComputedGroups
{
    public class DummyData
    {
        public int Data { get; set; }
    }
    
    public class TestComputedFromData : ComputedFromData<int, DummyData>
    {
        public Subject<bool> ManuallyRefresh = new Subject<bool>();
        
        public TestComputedFromData(DummyData data) : base(data)
        {}

        public override IObservable<bool> RefreshWhen()
        { return ManuallyRefresh; }

        public override int Transform(DummyData data)
        { return data.Data; }
    }
}