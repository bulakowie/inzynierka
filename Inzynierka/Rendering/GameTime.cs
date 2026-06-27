using System;


namespace Render
{
    public class GameTime
    {
        public TimeSpan TotalGameTime { get; set; }
        public TimeSpan ElapsedGameTime { get; set; }

        public GameTime()
        {
            TotalGameTime = TimeSpan.Zero;
            ElapsedGameTime = TimeSpan.Zero;

        }
        public GameTime(TimeSpan TotalGameTime, TimeSpan ElapsedGameTime)
        {
            this.TotalGameTime = TotalGameTime;
            this.ElapsedGameTime = ElapsedGameTime;
        }
    }
}