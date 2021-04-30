using System;
using SystemsRx.Attributes;
using EcsRx.Entities;
using EcsRx.Examples.Custom.Groups;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;

namespace EcsRx.Examples.Custom.Systems
{
    [Priority(100)]
    public class SecondSystem : ISetupSystem
    {
        public IGroup Group => new MessageGroup();

        public void Setup(IEntity entity)
        {
            Console.WriteLine("SYSTEM 2");
        }
    }
}