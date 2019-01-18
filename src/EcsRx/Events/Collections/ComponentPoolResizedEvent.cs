namespace EcsRx.Events.Collections
{
    public struct ComponentPoolResizedEvent
    {
        public readonly int ComponentTypeId;

        public ComponentPoolResizedEvent(int componentTypeId)
        {
            ComponentTypeId = componentTypeId;
        }
    }
}