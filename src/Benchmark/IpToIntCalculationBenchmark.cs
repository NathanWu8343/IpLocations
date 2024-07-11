using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class IpToIntCalculationBenchmark
    {
        private readonly long[] octets = new long[] { 127, 0, 0, 1 };

        //[GlobalSetup]
        //public void Setup()
        //{
        //}

        [Benchmark]
        public long Default()
        {
            return octets[0] * (256 * 256 * 256)
                   + octets[1] * (256 * 256)
                   + octets[2] * 256
                   + octets[3];
        }

        [Benchmark]
        public long LinqWithAggregate()
        {
            return octets.Aggregate((a, b) => a * 256 + b);
        }

        [Benchmark]
        public long LeftShitOperator()
        {
            return octets[0] << 24 | octets[1] << 16 | octets[2] << 8 | octets[3];
        }
    }
}