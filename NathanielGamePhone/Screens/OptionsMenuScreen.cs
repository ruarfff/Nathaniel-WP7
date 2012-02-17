namespace NathanielGame
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        readonly MenuEntry _soundMenuEntry;
        readonly MenuEntry _difficultyMenuEntry;

        enum Difficulty
        {
            Easy,
            Normal,
            Hard,
        }
        enum SoundSetting
        {
            On,
            Off,
        }
        private static Difficulty _currentDifficulty = Difficulty.Normal;
        private static SoundSetting _currentSoundSetting = SoundSetting.Off;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("")
        {
            // Create our menu entries.
            _soundMenuEntry = new MenuEntry(string.Empty);
            _difficultyMenuEntry = new MenuEntry(string.Empty);
            

            SetMenuEntryText();

            // Hook up menu event handlers.
            _soundMenuEntry.Selected += SoundMenuEntrySelected;
            _difficultyMenuEntry.Selected += DifficultyMenuEntrySelected;
           
            
            // Add entries to the menu.
            MenuEntries.Add(_soundMenuEntry);
            MenuEntries.Add(_difficultyMenuEntry);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            _difficultyMenuEntry.Text = "Difficulty level: " + _currentDifficulty;
            _soundMenuEntry.Text = "Sound: " + _currentSoundSetting;
            
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Diffigulty menu entry is selected.
        /// </summary>
        void DifficultyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _currentDifficulty++;

            if (_currentDifficulty > Difficulty.Hard)
                _currentDifficulty = 0;

            if(_currentDifficulty == Difficulty.Easy)
            {
                Player.difficulty = 0.5f;
            }
            else if(_currentDifficulty == Difficulty.Normal)
            {
                Player.difficulty = 1.0f;
            }
            else
            {
                Player.difficulty = 1.5f;
            }
            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the Sound menu entry is selected.
        /// </summary>
        void SoundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _currentSoundSetting++;
            if (_currentSoundSetting > SoundSetting.Off)
                _currentSoundSetting = 0;
            SetMenuEntryText();
            if (_currentSoundSetting == SoundSetting.Off)
            {
                Player.soundOff = true;
            }
            else
            {
                Player.soundOff = false;
            }
        }

        #endregion
    }
}
