using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using EcsRx.Benchmarks.Benchmarks;
using SystemsRx.Extensions;

namespace EcsRx.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var benchmarks = new []
            {
                //BenchmarkConverter.TypeToBenchmarks(typeof(EntityRetrievalBenchmark)),
                //BenchmarkConverter.TypeToBenchmarks(typeof(EntityAddComponentsBenchmark)),
                //BenchmarkConverter.TypeToBenchmarks(typeof(EntityGroupMatchingBenchmark)),
                //BenchmarkConverter.TypeToBenchmarks(typeof(ObservableGroupsAddAndRemoveBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(MultipleObservableGroupsAddAndRemoveBenchmark)),
            };
            
            var summaries = BenchmarkRunner.Run(benchmarks);
            var consoleLogger = ConsoleLogger.Default;
            consoleLogger.Flush();
            summaries.ForEachRun(x =>
            {
                AsciiDocExporter.Default.ExportToLog(x, consoleLogger);
                consoleLogger.WriteLine();
            });
        }
    }
}