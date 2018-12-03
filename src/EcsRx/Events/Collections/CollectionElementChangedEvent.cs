namespace EcsRx.Events.Collections
{
    public class CollectionElementChangedEvent<T>
    {
        public int Index { get; set; }
        public T OldValue { get; set; }
        public T NewValue { get; set; }        
    }
}