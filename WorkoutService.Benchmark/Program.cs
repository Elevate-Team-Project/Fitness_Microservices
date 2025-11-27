using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

namespace WorkoutService.Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = DefaultConfig.Instance
                .WithOptions(ConfigOptions.DisableOptimizationsValidator);

            var summary = BenchmarkRunner.Run<GetWorkoutDetailsBenchmark>(config);
        }
    }
}
