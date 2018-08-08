using System;

namespace CyberBiology.Core
{
    public static class Utils
    {
        private static readonly Random Random = new Random();

        public static int Next(int maxValue)
        {
            lock (Random)
            {
                return Random.Next(maxValue);
            }
        }

        public static double NextDouble()
        {
            lock (Random)
            {
                return Random.NextDouble();
            }
        }
    }
}
