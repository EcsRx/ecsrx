namespace EcsRx.Groups
{
    public class LookupGroup : ILookupGroup
    {
        public int[] RequiredComponents { get; }
        public int[] ExcludedComponents { get; }

        public LookupGroup(int[] requiredComponents, int[] excludedComponents)
        {
            RequiredComponents = requiredComponents;
            ExcludedComponents = excludedComponents;
        }
    }
}