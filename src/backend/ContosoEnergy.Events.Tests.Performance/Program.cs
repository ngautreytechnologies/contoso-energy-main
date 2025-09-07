// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using ContosoEnergy.Tests.Performance.Benchmarking;

Console.WriteLine("Hello, World!");

/// <summary>
/// Measure performance of job processing to ensure 
/// we meet performance expecations and agreements (SLAs e.g.).
BenchmarkRunner.Run<JobBenchmarks>();