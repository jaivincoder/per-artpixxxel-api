using System;


namespace api.artpixxel.repo.Utils.Generator.Util
{
    internal static class RandomUtils
    {
        private static readonly Random Random = new();
        private static readonly object ThreadLock = new();

        public static int GenerateNumberInRange(int min, int max)
        {
            lock (ThreadLock)
            {
                return Random.Next(min, max);
            }
        }
    }
}
