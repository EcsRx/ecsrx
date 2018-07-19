using System;
using System.Reactive.Linq;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.Components;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;

namespace EcsRx.Examples.ExampleApps.ComputedGroupExample.Systems
{
    public class RandomlyChangeHpSystem : IReactToGroupSystem
    {
        private const int HealthChange = 20;
        
        public IGroup Group { get; } = new Group(typeof(HasHealthComponent));
        private Random _random = new Random();
        
        public IObservable<IObservableGroup> ReactToGroup(IObservableGroup observableGroup)
        { return Observable.Interval(TimeSpan.FromMilliseconds(500)).Select(x => observableGroup); }

        public void Process(IEntity entity)
        {
            var healthComponent = entity.GetComponent<HasHealthComponent>();

            var healthChange = CreateRandomHealthChange();
            healthComponent.CurrentHealth += healthChange;

            if (healthComponent.CurrentHealth <= 0 || healthComponent.CurrentHealth > healthComponent.MaxHealth)
            { healthComponent.CurrentHealth = healthComponent.MaxHealth; }            
        }

        public int CreateRandomHealthChange()
        { return _random.Next(-HealthChange, 0); }
    }
}