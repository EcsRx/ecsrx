﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using EcsRx.Computeds.Groups;
using SystemsRx.Extensions;
using SystemsRx.Systems;
using SystemsRx.Systems.Conventional;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.ComputedGroups;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.Extensions;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;

namespace EcsRx.Examples.ExampleApps.ComputedGroupExample.Systems
{
    public class DisplayLowestHealthSystem : IManualSystem
    {
        private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();

        private readonly ILowestHealthComputedGroup _lowestHealthGroup;

        public DisplayLowestHealthSystem(ILowestHealthComputedGroup lowestHealthGroup)
        {
            _lowestHealthGroup = lowestHealthGroup;
        }

        public void StartSystem()
        { Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(UpdateListings).AddTo(_subscriptions); }
        
        public void StopSystem()
        { _subscriptions.DisposeAll(); }

        public void UpdateListings(long _)
        {
            Console.SetCursorPosition(0,0);
            Console.Clear();

            Console.WriteLine(" == All Characters HP == ");
            foreach (var entity in ((ComputedGroup)_lowestHealthGroup).InternalObservableGroup)
            { Console.WriteLine($"{entity.GetName()} - {entity.GetHealthPercentile()}% hp ({entity.GetHealthString()})"); }

            Console.WriteLine();
            
            var position = 1;
            Console.WriteLine(" == Characters With HP < 50% == ");
            foreach (var entity in _lowestHealthGroup)
            { Console.WriteLine($"{position++}) {entity.GetName()} - {entity.GetHealthPercentile()}% hp ({entity.GetHealthString()})"); }
        }
    }
}