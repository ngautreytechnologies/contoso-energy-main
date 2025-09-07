using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Linq;

namespace ContosoEnergy.Tests.Performance.Benchmarking
{
    public class JobBenchmarks
    {
        private int[]? numbers;

        [GlobalSetup]
        public void Setup()
        {
            numbers = [.. Enumerable.Range(1, 1000)];
        }

        [Benchmark]
        public int SumWithForLoop()
        {
            if (numbers == null)
                throw new InvalidOperationException("numbers is not initialized.");

            int sum = 0;
            for (int i = 0; i < numbers.Length; i++)
                sum += numbers[i];
            return sum;
        }

        [Benchmark]
        public int SumWithLinq()
        {
            if (numbers == null)
                throw new InvalidOperationException("numbers is not initialized.");

            return numbers.Sum();
        }
    }
}
