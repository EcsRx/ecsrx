namespace EcsRx.Events.Collections
{
    public struct CollectionElementChangedEvent<T>
    {
        public int Index;
        public T OldValue;
        public T NewValue;
    }
}