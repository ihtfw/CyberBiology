namespace CyberBiology.Core.Enums
{
    public enum BotState
    {
        /// <summary>
        /// место свободно, здесь может быть размещен новый бот
        /// </summary>
        Free = 0,
        /// <summary>
        /// бот погиб и представляет из себя органику в подвешенном состоянии
        /// </summary>
        OrganicHold = 1,
        /// <summary>
        /// ораника начинает тонуть, пока не встретит препятствие, после чего остается в подвешенном состоянии(LV_ORGANIC_HOLD)
        /// </summary>
        OrganicSink = 2,
        /// <summary>
        ///  живой бот
        /// </summary>
        Alive = 3
    }
}