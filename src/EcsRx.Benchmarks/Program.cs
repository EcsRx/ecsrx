using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using EcsRx.Benchmarks.Benchmarks;

namespace EcsRx.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run(new []
            {
                //BenchmarkConverter.TypeToBenchmarks(typeof(EntityRetrievalBenchmark)),
                //BenchmarkConverter.TypeToBenchmarks(typeof(EntityAddComponentsBenchmark)),
                //BenchmarkConverter.TypeToBenchmarks(typeof(EntityGroupMatchingBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(ObservableGroupsAddAndRemoveBenchmark)),
            });
        }
    }
}