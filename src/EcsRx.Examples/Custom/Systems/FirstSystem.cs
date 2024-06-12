﻿using System;
using SystemsRx.Attributes;
using EcsRx.Entities;
using EcsRx.Examples.Custom.Groups;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Examples.Custom.Systems
{
    [Priority(10)]
    public class FirstSystem : ISetupSystem
    {
        public IGroup Group => new MessageGroup();

        public void Setup(IEntity entity)
        {
            Console.WriteLine("SYSTEM 1");
        }
    }
}