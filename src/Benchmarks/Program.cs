// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Benchmarks.Core.Collections;

Console.WriteLine("Hello, World!");
var summary = BenchmarkRunner.Run<ConcurrentBTreeDictionaryBenchmarks>();
