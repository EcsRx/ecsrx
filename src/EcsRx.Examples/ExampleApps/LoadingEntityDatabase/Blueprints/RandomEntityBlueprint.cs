using System;
using System.Linq;
using System.Numerics;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Components;
using EcsRx.Extensions;

namespace EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Blueprints
{
    public class RandomEntityBlueprint : IBlueprint
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private Random _random = new Random();
        
        public void Apply(IEntity entity)
        {
            var component1 = new DummyComponent1
            {
                SomeNumber = _random.Next(),
                SomeString = RandomString(),
                SomeTime = DateTime.FromBinary(_random.Next(int.MaxValue))
            };

            var component2 = new DummyComponent2
            {
                SomeVector = RandomVector(),
                SomeQuaternion = RandomQuaternion()
            };
            
            entity.AddComponents(component1, component2);
        }
        
        public string RandomString()
        {
            var length = _random.Next(2, 24);
            var randomness = Enumerable
                .Repeat(Chars, length)
                .Select(s => s[_random.Next(s.Length)])
                .ToArray();
            
            return new string(randomness);
        }

        public Vector3 RandomVector()
        {
            return new Vector3(
                (float)_random.NextDouble(), 
                (float)_random.NextDouble(),
                (float)_random.NextDouble());
        }
        
        public Quaternion RandomQuaternion()
        { return new Quaternion(RandomVector(), (float)_random.NextDouble()); }
    }
}