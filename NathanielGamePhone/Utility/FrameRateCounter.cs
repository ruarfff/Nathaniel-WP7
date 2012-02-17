using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace NathanielGame
{
    static class FrameRateCounter
    {
       
        static int _frameRate;
        static int _frameCounter;
        static TimeSpan _elapsedTime;
        public static void Initialize()
        {
            _frameRate = 0;
            _frameCounter = 0;
            _elapsedTime = TimeSpan.Zero;
        }

        public static void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }
        }


        public static void Draw(GameTime gameTime)
        {
            _frameCounter++;

            string fps = string.Format("fps: {0}", _frameRate);

            Debug.WriteLine(fps);
        }
    }
}
