// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Benchmarks.Core.Collections;
using Reko.Benchmarks.Arch.X86;

//var summary = BenchmarkRunner.Run<ConcurrentBTreeDictionaryBenchmarks>();
var summary = BenchmarkRunner.Run<X86DisassemblerBenchmarks>();
summary = BenchmarkRunner.Run<X86RewriterBenchmarks>();
