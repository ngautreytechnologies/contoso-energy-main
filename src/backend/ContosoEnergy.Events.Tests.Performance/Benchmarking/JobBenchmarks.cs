using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ContosoEnergy.Tests.Performance.Benchmarking
{
    // Measure memory allocations
    [MemoryDiagnoser]
    public class JobBenchmarks
    {
        private readonly ILogger<JobBenchmarks> _logger;
        private List<string> _telemetryLogs;

        public JobBenchmarks()
        {
            _telemetryLogs = new List<string>();
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<JobBenchmarks>();
        }

        [GlobalSetup]
        public void Setup()
        {
            // Simulate some initialization needed before benchmarks
            _logger.LogInformation("Benchmark setup complete.");
        }

        // NFR: Should create a job under 50ms on average
        [Benchmark]
        public void CreateJob_PerformanceTest()
        {
            var start = DateTime.UtcNow;

            // Simulate job creation
            SimulateJobCreation();

            var elapsed = DateTime.UtcNow - start;

            _logger.LogInformation("CreateJob took {ElapsedMs} ms", elapsed.TotalMilliseconds);

            _telemetryLogs.Add($"CreateJob duration: {elapsed.TotalMilliseconds} ms");

            // Raise warning if performance NFR is violated
            // TODO: Make this configurable via appsettings or environment variable
            if (elapsed.TotalMilliseconds > 50)
            {
                _logger.LogWarning("Performance NFR violated: CreateJob took longer than 50ms!");
            }
        }

        // NFR: Memory usage should stay under 10kb per job
        [Benchmark]
        public void CreateJob_MemoryUsageTest()
        {
            long memoryBefore = GC.GetTotalMemory(true);

            SimulateJobCreation();

            long memoryAfter = GC.GetTotalMemory(true);
            long delta = memoryAfter - memoryBefore;
            _logger.LogInformation("CreateJob memory delta: {DeltaBytes} bytes", delta);
            _telemetryLogs.Add($"Memory delta: {delta} bytes");

            // Raise warning if memory NFR is violated
            if (delta > 10_000)
            {
                _logger.LogWarning("Memory NFR violated: CreateJob allocated more than 10kb!");
            }
        }

        // NFR: Telemetry/logs must be emitted
        [Benchmark]
        public void CreateJob_TelemetryTest()
        {
            SimulateJobCreation();

            if (_telemetryLogs.Count == 0)
            {
                _logger.LogWarning("Telemetry NFR violated: No telemetry emitted for CreateJob!");
            }
            else
            {
                _logger.LogInformation("Telemetry emitted successfully for CreateJob.");
            }
        }

        // -------------------------
        // Helper: Simulated job creation
        // -------------------------
        private void SimulateJobCreation()
        {
            // Fake some processing
            Thread.Sleep(5); // simulate work

            // Fake log/telemetry
            _telemetryLogs.Add("Job created successfully.");
            _logger.LogDebug("Simulated job creation complete.");
        }
    }
}
