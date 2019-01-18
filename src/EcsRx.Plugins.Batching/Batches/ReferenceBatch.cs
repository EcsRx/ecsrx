using EcsRx.Components;

namespace EcsRx.Plugins.Batching.Batches
{   
    public class ReferenceBatch<T1, T2>
        where T1 : class, IComponent
        where T2 : class, IComponent
    {
        public readonly int EntityId;
        public readonly T1 Component1;
        public readonly T2 Component2;

        public ReferenceBatch(int entityId, T1 component1, T2 component2)
        {
            EntityId = entityId;
            Component1 = component1;
            Component2 = component2;
        }
    }
    
    public class ReferenceBatch<T1, T2, T3>
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
    {
        public readonly int EntityId;
        public readonly T1 Component1;
        public readonly T2 Component2;
        public readonly T3 Component3;

        public ReferenceBatch(int entityId, T1 component1, T2 component2, T3 component3)
        {
            EntityId = entityId;
            Component1 = component1;
            Component2 = component2;
            Component3 = component3;
        }
    }
    
    public class ReferenceBatch<T1, T2, T3, T4>
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
    {
        public readonly int EntityId;
        public readonly T1 Component1;
        public readonly T2 Component2;
        public readonly T3 Component3;
        public readonly T4 Component4;

        public ReferenceBatch(int entityId, T1 component1, T2 component2, T3 component3, T4 component4)
        {
            EntityId = entityId;
            Component1 = component1;
            Component2 = component2;
            Component3 = component3;
            Component4 = component4;
        }
    }
    
    public class ReferenceBatch<T1, T2, T3, T4, T5>
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
    {
        public readonly int EntityId;
        public readonly T1 Component1;
        public readonly T2 Component2;
        public readonly T3 Component3;
        public readonly T4 Component4;
        public readonly T5 Component5;

        public ReferenceBatch(int entityId, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5)
        {
            EntityId = entityId;
            Component1 = component1;
            Component2 = component2;
            Component3 = component3;
            Component4 = component4;
            Component5 = component5;
        }
    }
    
    public class ReferenceBatch<T1, T2, T3, T4, T5, T6>
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
        where T6 : class, IComponent
    {
        public readonly int EntityId;
        public readonly T1 Component1;
        public readonly T2 Component2;
        public readonly T3 Component3;
        public readonly T4 Component4;
        public readonly T5 Component5;
        public readonly T6 Component6;

        public ReferenceBatch(int entityId, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6)
        {
            EntityId = entityId;
            Component1 = component1;
            Component2 = component2;
            Component3 = component3;
            Component4 = component4;
            Component5 = component5;
            Component6 = component6;
        }
    }
}