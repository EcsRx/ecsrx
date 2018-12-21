namespace EcsRx.Groups.Batches
{
    public interface IComponentBatches
    {}
    
    public interface IComponentBatches<out T> : IComponentBatches where T : IBatchDescriptor
    {
        T[] Batches { get; }
    }
}