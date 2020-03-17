using System;
using EcsRx.Components;

namespace EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Components
{
    public class DummyComponent1 : IComponent
    {
        public int SomeNumber { get; set; }
        public string SomeString { get; set; }
        public DateTime SomeTime { get; set; }
    }
}