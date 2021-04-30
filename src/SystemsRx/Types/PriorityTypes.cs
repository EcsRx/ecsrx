namespace SystemsRx.Types
{
    /// <summary>
    /// Default priority values incase you do not want to 
    /// </summary>
    /// <remarks>
    /// The ordering will be from highest to lowest so if you have a priority of 10
    /// it will load before a system with a priority of 1. You can also use negative
    /// priorities if you want something to run AFTER the defaults, so -1 would run
    /// after default then -100 would run after that, making the order:
    /// 100, 1, 0, -1, -100
    /// </remarks>
    public class PriorityTypes
    {
        public const int SuperLow = -1000;
        public const int Lower = -100;
        public const int Low = -10;
        public const int Default = 0;
        public const int High = 10;
        public const int Higher = 100;
        public const int SuperHigh = 1000;
    }
}