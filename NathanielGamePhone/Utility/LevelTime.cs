using System;
using System.Diagnostics;

namespace NathanielGame
{
    public static class LevelTime
    {
        private static Stopwatch _levelTimer;

        public static TimeSpan CurrentTime
        {
            get
            {
                if(_levelTimer != null)
                return _levelTimer.Elapsed;

                return TimeSpan.Zero;
            }
        }


        public static void Initialize()
        {
            _levelTimer = new Stopwatch();
            _levelTimer.Start();
        }

        public static void Pause()
        {
            _levelTimer.Stop();
        }

        public static void Resume()
        {
            _levelTimer.Start();
        }

        public static void Reset()
        {
            _levelTimer.Reset();
        }
    }
}
