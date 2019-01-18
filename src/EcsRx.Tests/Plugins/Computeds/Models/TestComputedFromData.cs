using System;
using EcsRx.MicroRx.Subjects;
using EcsRx.Plugins.Computeds;

namespace EcsRx.Tests.Plugins.Computeds.Models
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