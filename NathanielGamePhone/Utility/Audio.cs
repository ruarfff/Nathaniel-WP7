using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace NathanielGame
{
    class Audio
    {
        #region Sound Effects
        // The sound that is played when a laser is fired
        private static SoundEffect _laserSound;
        public static void LaserSound()
        {
            if (!Player.soundOff)
            _laserSound.Play(); 
        }


        // The sound used when the player or an enemy dies
        private static SoundEffect _explosionSound;
        public static void ExplosionSound()
        {
            if (!Player.soundOff)
            _explosionSound.Play();
        }

        private static SoundEffect _gunShot;
        public static void GunShot()
        {
            if (!Player.soundOff)
            _gunShot.Play();
        }

        private static SoundEffect _laserCannon;
        public static void LaserCannon()
        {
            if (!Player.soundOff)
            _laserCannon.Play();
        }

        private static SoundEffect _evilLaugh;
        public static void EvilLaugh()
        {
            if (!Player.soundOff)
            _evilLaugh.Play();
        }

        private static SoundEffect _rayGun;
        public static void RayGun()
        {
            if (!Player.soundOff)
            _rayGun.Play(); 
        }

        private static SoundEffect _arrowShot;
        public static void ArrowShot()
        {
            if (!Player.soundOff)
            _arrowShot.Play(); 
        }

        private static SoundEffect _laserBlast;
        public static void LaserBlast()
        {
            if (!Player.soundOff)
            _laserBlast.Play();
        }

        #endregion

        #region Music
        private static Song _menuMusic;
        public static Song MenuMusic
        {
            get { return _menuMusic; }
        }
        // The music played during gameplay
        private static Song _gameplayMusic;
        public static Song GamePlayMusic
        {
            get { return _gameplayMusic; }
        }
        #endregion

        public static void LoadSounds(ContentManager content)
        {
            try
            {
                // Load the music
                _menuMusic = content.Load<Song>("Sounds/menuMusic");
                _gameplayMusic = content.Load<Song>("Sounds/gameMusic");
                // Load the sound effect
                _laserSound = content.Load<SoundEffect>("Sounds/laserFire");
                _explosionSound = content.Load<SoundEffect>("Sounds/explosion");
                _gunShot = content.Load<SoundEffect>("Sounds/gunShot");
                _rayGun = content.Load<SoundEffect>("Sounds/rayGun");
                _laserCannon = content.Load<SoundEffect>("Sounds/laserCannon");
                _evilLaugh = content.Load<SoundEffect>("Sounds/evilLaugh");
                _arrowShot = content.Load<SoundEffect>("Sounds/arrowShot");
                _laserBlast = content.Load<SoundEffect>("Sounds/zap");
            }
            catch
            {
                //Empty
            }
        }

        public static void PlayMusic(Song song)
        {
            if (!Player.soundOff)
            {
                // Due to the way the MediaPlayer plays music,
                // we have to catch the exception. Music will play when the game is not tethered
                try
                {
                    // Play the music
                    MediaPlayer.Play(song);


                    // Loop the currently playing song
                    MediaPlayer.IsRepeating = true;

                    MediaPlayer.Volume = 1.0f;
                }
                catch
                {
                    //Empty
                }
            }
        }

        public static void StopMusic()
        {
            try
            {
                if ((MediaPlayer.State == MediaState.Playing) || (MediaPlayer.State == MediaState.Paused))
                    MediaPlayer.Stop();
            }
            catch
            {
                //Empty
            }
        }

        public static void PauseMusic()
        {
            try
            {
                if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Pause();
            }
            catch
            {
                //Empty
            }
        }

        public static void ResumeMusic()
        {
            try
            {
                if (!Player.soundOff && MediaPlayer.State == MediaState.Paused)
                    MediaPlayer.Resume();
            }
            catch
            {
                //Empty
            }
        }

    }
}
