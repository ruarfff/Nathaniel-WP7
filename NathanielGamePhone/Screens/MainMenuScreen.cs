#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#endregion

namespace NathanielGame
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("")
        {
           
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("New Game");
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry survivalMenuEntry = new MenuEntry("Survival");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry highScoresMenuEntry = new MenuEntry("High Scores");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            resumeGameMenuEntry.Selected += ResumeGameMenuEntrySelected;
            survivalMenuEntry.Selected += SurvivalMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            highScoresMenuEntry.Selected += HighScoresMenuEntrySelected;


            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(survivalMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(highScoresMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new InstructionScreen(e,GameToPlay.NewGame), e.PlayerIndex);
        }

        void ResumeGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(GameToPlay.LoadGame));
        }

        void SurvivalMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(GameToPlay.SurvivalGame));
        }
        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        void HighScoresMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            HighScores.UploadScore();
            ScreenManager.AddScreen(new HighScoresScreen(), e.PlayerIndex);
        }
        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
