using System;
using EcsRx.Computed;
using EcsRx.MicroRx.Subjects;

namespace EcsRx.Tests.Computeds
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