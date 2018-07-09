using System;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Performance.Components;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Examples.ExampleApps.Performance.Systems
{
    public class EntityUpdateSystem : IReactToEntitySystem
    {
        public IGroup TargetGroup => new Group(
            typeof(Component1),
            typeof(Component2),
            typeof(Component3),
            typeof(Component4),
            typeof(Component5),
            typeof(Component6),
            typeof(Component7),
            typeof(Component8),
            typeof(Component9),
            typeof(Component10),
            typeof(Component11),
            typeof(Component12),
            typeof(Component13),
            typeof(Component14),
            typeof(Component15),
            typeof(Component16),
            typeof(Component17),
            typeof(Component18),
            typeof(Component19),
            typeof(Component20)
        );

        public IObservable<IEntity> ReactToEntity(IEntity entity)
        {
            return null;
        }

        public void Execute(IEntity entity)
        {
            var component1 = entity.GetComponent<Component1>();
            var component2 = entity.GetComponent<Component2>();
            var component3 = entity.GetComponent<Component3>();
            var component4 = entity.GetComponent<Component4>();
            var component5 = entity.GetComponent<Component5>();
            var component6 = entity.GetComponent<Component6>();
            var component7 = entity.GetComponent<Component7>();
            var component8 = entity.GetComponent<Component8>();
            var component9 = entity.GetComponent<Component9>();
            var component10 = entity.GetComponent<Component10>();
            var component11 = entity.GetComponent<Component11>();
            var component12 = entity.GetComponent<Component12>();
            var component13 = entity.GetComponent<Component13>();
            var component14 = entity.GetComponent<Component14>();
            var component15 = entity.GetComponent<Component15>();
            var component16 = entity.GetComponent<Component16>();
            var component17 = entity.GetComponent<Component17>();
            var component18 = entity.GetComponent<Component18>();
            var component19 = entity.GetComponent<Component19>();
            var component20 = entity.GetComponent<Component20>();

            // Stop optimizing away the usages
            if(component1 == null) { }
            if(component2 == null) { }
            if(component3 == null) { }
            if(component4 == null) { }
            if(component5 == null) { }
            if(component6 == null) { }
            if(component7 == null) { }
            if(component8 == null) { }
            if(component9 == null) { }
            if(component10 == null) { }
            if(component11 == null) { }
            if(component12 == null) { }
            if(component13 == null) { }
            if(component14 == null) { }
            if(component15 == null) { }
            if(component16 == null) { }
            if(component17 == null) { }
            if(component18 == null) { }
            if(component19 == null) { }
            if(component20 == null) { }
        }
    }
}