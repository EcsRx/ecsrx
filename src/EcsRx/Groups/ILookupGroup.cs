namespace EcsRx.Groups
{
    public interface ILookupGroup
    {
        int[] RequiredComponents { get; }
        int[] ExcludedComponents { get; }
    }
}