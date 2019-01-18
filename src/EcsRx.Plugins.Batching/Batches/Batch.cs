using System.Runtime.InteropServices;
using EcsRx.Components;

namespace EcsRx.Plugins.Batching.Batches
{      
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Batch<T1, T2>
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
    {
        public readonly int EntityId;
        public readonly T1* Component1;
        public readonly T2* Component2;

        public Batch(int entityId, T1* component1, T2* component2)
        {
            EntityId = entityId;
            Component1 = component1;
            Component2 = component2;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Batch<T1, T2, T3>
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
    {
        public readonly int EntityId;
        public readonly T1* Component1;
        public readonly T2* Component2;
        public readonly T3* Component3;

        public Batch(int entityId, T1* component1, T2* component2, T3* component3)
        {
            EntityId = entityId;
            Component1 = component1;
            Component2 = component2;
            Component3 = component3;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Batch<T1, T2, T3, T4>
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
    {
        public readonly int EntityId;
        public readonly T1* Component1;
        public readonly T2* Component2;
        public readonly T3* Component3;
        public readonly T4* Component4;

        public Batch(int entityId, T1* component1, T2* component2, T3* component3, T4* component4)
        {
            EntityId = entityId;
            Component1 = component1;
            Component2 = component2;
            Component3 = component3;
            Component4 = component4;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Batch<T1, T2, T3, T4, T5>
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
    {
        public readonly int EntityId;
        public readonly T1* Component1;
        public readonly T2* Component2;
        public readonly T3* Component3;
        public readonly T4* Component4;
        public readonly T5* Component5;

        public Batch(int entityId, T1* component1, T2* component2, T3* component3, T4* component4, T5* component5)
        {
            EntityId = entityId;
            Component1 = component1;
            Component2 = component2;
            Component3 = component3;
            Component4 = component4;
            Component5 = component5;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Batch<T1, T2, T3, T4, T5, T6>
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
        where T6 : unmanaged, IComponent
    {
        public readonly int EntityId;
        public readonly T1* Component1;
        public readonly T2* Component2;
        public readonly T3* Component3;
        public readonly T4* Component4;
        public readonly T5* Component5;
        public readonly T6* Component6;

        public Batch(int entityId, T1* component1, T2* component2, T3* component3, T4* component4, T5* component5, T6* component6)
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