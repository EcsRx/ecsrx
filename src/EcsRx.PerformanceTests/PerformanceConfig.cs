using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace EcsRx.PerformanceTests
{
    public class PerformanceConfig : ManualConfig
    {
        public PerformanceConfig()
        {
            Add(MarkdownExporter.GitHub);
            Add(MemoryDiagnoser.Default);

            var baseConfig = Job.ShortRun.WithLaunchCount(1).WithTargetCount(1).WithWarmupCount(1);
            Add(baseConfig.With(Runtime.Core).With(Platform.X64));
        }
    }
}