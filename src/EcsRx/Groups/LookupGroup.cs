namespace EcsRx.Groups
{
    public struct LookupGroup
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