using System;

namespace NJG.Runtime.Map
{
    public class DeterministicRandom
    {
        private Random _random;

        public DeterministicRandom(int seed)
        {
            _random = new Random(seed);
        }

        public void Reset(int seed)
        {
            _random = new Random(seed);
        }

        public int Range(int minInclusive, int maxExclusive)
        {
            return _random.Next(minInclusive, maxExclusive);
        }

        public float Range(float minInclusive, float maxInclusive)
        {
            return (float)(minInclusive + _random.NextDouble() * (maxInclusive - minInclusive));
        }

        public bool Chance(float chance)
        {
            return Range(0f, 1f) < chance;
        }
    }
}