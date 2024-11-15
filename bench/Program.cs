using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using EtherSharp.Bench;

BenchmarkRunner.Run<Keccak256Benchmarks>();
