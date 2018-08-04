using System.Collections.Concurrent;

namespace CyberBiology.Core
{
    public static class BotFactory
    {
        private static readonly ConcurrentQueue<Bot> Queue = new ConcurrentQueue<Bot>();

        public static Bot Get(int x, int y)
        {
            if (Queue.TryDequeue(out var bot))
            {
                bot.X = x;
                bot.Y = y;
            }
            else
            {
                bot = new Bot(x, y);
            }

            return bot;
        }

        public static void ToCache(Bot bot)
        {
            Queue.Enqueue(bot);
        }
    }
}
