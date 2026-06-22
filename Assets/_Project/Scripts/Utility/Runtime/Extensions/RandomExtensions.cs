namespace NJG.Utilities
{
    public static class RandomExtensions
    {
        public static bool Chance(this System.Random randomizer, float probability)
        {
            return randomizer == null ? false : randomizer.NextDouble() <= probability;
        }
    }
}