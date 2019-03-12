using System;
using System.Runtime.InteropServices;
using EcsRx.Components;

namespace EcsRx.Plugins.Batching.Batches
{      
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Batch<T1, T2> : IEquatable<Batch<T1, T2>> 
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
    {
        public readonly int EntityId;
        public readonly T1* Component1;
        public readonly T2* Component2;

        public bool Equals(Batch<T1, T2> other)
        { return EntityId == other.EntityId && Component1 == other.Component1 && Component2 == other.Component2; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            return obj is Batch<T1, T2> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EntityId;
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component1);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component2);
                return hashCode;
            }
        }

        public Batch(int entityId, T1* component1, T2* component2)
        {
            EntityId = entityId;
            Component1 = component1;
            Component2 = component2;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Batch<T1, T2, T3> : IEquatable<Batch<T1, T2, T3>> 
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
    {
        public readonly int EntityId;
        public readonly T1* Component1;
        public readonly T2* Component2;
        public readonly T3* Component3;

        public bool Equals(Batch<T1, T2, T3> other)
        {
            return EntityId == other.EntityId && Component1 == other.Component1 && 
                   Component2 == other.Component2 && Component3 == other.Component3;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            return obj is Batch<T1, T2, T3> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EntityId;
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component1);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component2);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component3);
                return hashCode;
            }
        }

        public Batch(int entityId, T1* component1, T2* component2, T3* component3)
        {
            EntityId = entityId;
            Component1 = component1;
            Component2 = component2;
            Component3 = component3;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Batch<T1, T2, T3, T4> : IEquatable<Batch<T1, T2, T3, T4>> 
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

        public bool Equals(Batch<T1, T2, T3, T4> other)
        {
            return EntityId == other.EntityId && Component1 == other.Component1 && 
                   Component2 == other.Component2 && Component3 == other.Component3 && 
                   Component4 == other.Component4;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            return obj is Batch<T1, T2, T3, T4> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EntityId;
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component1);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component2);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component3);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component4);
                return hashCode;
            }
        }

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
    public unsafe struct Batch<T1, T2, T3, T4, T5> : IEquatable<Batch<T1, T2, T3, T4, T5>> 
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

        public bool Equals(Batch<T1, T2, T3, T4, T5> other)
        {
            return EntityId == other.EntityId && Component1 == other.Component1 && 
                   Component2 == other.Component2 && Component3 == other.Component3 && 
                   Component4 == other.Component4 && Component5 == other.Component5;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            return obj is Batch<T1, T2, T3, T4, T5> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EntityId;
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component1);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component2);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component3);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component4);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component5);
                return hashCode;
            }
        }

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
    public unsafe struct Batch<T1, T2, T3, T4, T5, T6> : IEquatable<Batch<T1, T2, T3, T4, T5, T6>> 
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

        public bool Equals(Batch<T1, T2, T3, T4, T5, T6> other)
        {
            return EntityId == other.EntityId && Component1 == other.Component1 && 
                   Component2 == other.Component2 && Component3 == other.Component3 && 
                   Component4 == other.Component4 && Component5 == other.Component5 && 
                   Component6 == other.Component6;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            return obj is Batch<T1, T2, T3, T4, T5, T6> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EntityId;
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component1);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component2);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component3);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component4);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component5);
                hashCode = (hashCode * 397) ^ unchecked((int) (long) Component6);
                return hashCode;
            }
        }

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