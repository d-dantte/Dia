// See https://aka.ms/new-console-template for more information
using Axis.Dia.Benchmark.Tests;
using BenchmarkDotNet.Running;

_ = BenchmarkRunner.Run<SerializationTests>();