namespace EcsRx.Components
{
    /// <summary>
    /// A container for isolated contextual data on an entity, should never contain logic.
    /// </summary>
    /// <remarks>
    /// Components should contain pure data which is passed around to different systems.
    /// If you also need to dispose on data inside your component i.e ReactiveProperty vars
    /// then just implement IDisposable as well and they will be auto disposed when the entity
    /// disposes
    /// </remarks>
    public interface IComponent
    {

    }
}
