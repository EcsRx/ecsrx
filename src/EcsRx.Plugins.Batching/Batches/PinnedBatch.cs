using System;
using System.Runtime.InteropServices;
using EcsRx.Components;

namespace EcsRx.Plugins.Batching.Batches
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PinnedBatch<T1, T2> : IDisposable
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
    {
        public readonly Batch<T1, T2>[] Batches;
        public readonly GCHandle[] Handles;
        
        public PinnedBatch(Batch<T1, T2>[] batches, GCHandle[] handles)
        {
            Batches = batches;
            Handles = handles;
        }

        public void Dispose()
        {
            if (Handles == null) { return; }
            foreach (var handle in Handles)
            {
                if(handle.IsAllocated)
                { handle.Free(); }
            }
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct PinnedBatch<T1, T2, T3> : IDisposable
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
    {
        public readonly Batch<T1, T2, T3>[] Batches;
        public readonly GCHandle[] Handles;
        
        public PinnedBatch(Batch<T1, T2, T3>[] batches, GCHandle[] handles)
        {
            Batches = batches;
            Handles = handles;
        }

        public void Dispose()
        {
            if (Handles == null) { return; }
            foreach (var handle in Handles)
            {
                if(handle.IsAllocated)
                { handle.Free(); }
            }
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct PinnedBatch<T1, T2, T3, T4> : IDisposable
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
    {
        public readonly Batch<T1, T2, T3, T4>[] Batches;
        public readonly GCHandle[] Handles;
        
        public PinnedBatch(Batch<T1, T2, T3, T4>[] batches, GCHandle[] handles)
        {
            Batches = batches;
            Handles = handles;
        }

        public void Dispose()
        {
            if (Handles == null) { return; }
            foreach (var handle in Handles)
            {
                if(handle.IsAllocated)
                { handle.Free(); }
            }
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct PinnedBatch<T1, T2, T3, T4, T5> : IDisposable
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
    {
        public readonly Batch<T1, T2, T3, T4, T5>[] Batches;
        public readonly GCHandle[] Handles;
        
        public PinnedBatch(Batch<T1, T2, T3, T4, T5>[] batches, GCHandle[] handles)
        {
            Batches = batches;
            Handles = handles;
        }

        public void Dispose()
        {
            if (Handles == null) { return; }
            foreach (var handle in Handles)
            {
                if(handle.IsAllocated)
                { handle.Free(); }
            }
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct PinnedBatch<T1, T2, T3, T4, T5, T6> : IDisposable
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
        where T6 : unmanaged, IComponent
    {
        public readonly Batch<T1, T2, T3, T4, T5, T6>[] Batches;
        public readonly GCHandle[] Handles;
        
        public PinnedBatch(Batch<T1, T2, T3, T4, T5, T6>[] batches, GCHandle[] handles)
        {
            Batches = batches;
            Handles = handles;
        }

        public void Dispose()
        {
            if (Handles == null) { return; }
            foreach (var handle in Handles)
            {
                if(handle.IsAllocated)
                { handle.Free(); }
            }
        }
    }
}