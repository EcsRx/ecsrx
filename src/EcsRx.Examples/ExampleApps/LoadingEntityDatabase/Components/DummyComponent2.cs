using System.Numerics;
using EcsRx.Components;

namespace EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Components
{
    public class DummyComponent2 : IComponent
    {
        public Vector3 SomeVector { get; set; }
        public Quaternion SomeQuaternion { get; set; }
    }
}