namespace SystemsRx.Systems.Conventional
{
    /// <summary>
    /// IReactToEventSystem are specifically made to act as event handlers,
    /// so when the given event comes in the system processes the event.
    /// </summary>
    /// <typeparam name="T">The type of event to handle</typeparam>
    public interface IReactToEventSystem<T> : ISystem
    {
        void Process(T eventData);
    }
}