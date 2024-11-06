using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using EtherSharp.Bench;

BenchmarkRunner.Run<ABIDecoderBenchmarks>(
    DefaultConfig.Instance.AddJob(Job
         .ShortRun
         .WithLaunchCount(1)
         .WithToolchain(InProcessEmitToolchain.Instance)));
