#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#endregion

namespace NathanielGame
{
    /// <summary>
    /// </summary>
    public class NathanielGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly ScreenManager _screenManager;
        public ContentManager StaticContent { get; set; }
        #region Initialization

        /// <summary>
        /// The main game constructor.
        /// </summary>
        public NathanielGame()
        {
            _graphics = new GraphicsDeviceManager(this) {IsFullScreen = true};
            if (Services != null) StaticContent = new ContentManager(Services);
            TargetElapsedTime = TimeSpan.FromTicks(333333);
            // configure the content manager
            Content.RootDirectory = "Content";
            StaticContent.RootDirectory = "Content";

             InitializeLandscapeGraphics();
            //InitializePortraitGraphics();

            // Create the screen manager component.
            _screenManager = new ScreenManager(this);

            Components.Add(_screenManager);

            // attempt to deserialize the screen manager from disk. if that
            // fails, we add our default screens.
            if (_screenManager.DeserializeState()) return;
            // Activate the first screens.
            _screenManager.AddScreen(new BackgroundScreen(), null);
            _screenManager.AddScreen(new MainMenuScreen(), null);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            // serialize the screen manager whenever the game exits
            _screenManager.SerializeState();

            base.OnExiting(sender, args);
        }

        /// <summary>
        /// Helper method to the initialize the game to be a portrait game.
        /// </summary>
        private void InitializePortraitGraphics()
        {
            _graphics.PreferredBackBufferWidth = 480;
            _graphics.PreferredBackBufferHeight = 800;
        }

        /// <summary>
        /// Helper method to initialize the game to be a landscape game.
        /// </summary>
        private void InitializeLandscapeGraphics()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }

        #endregion
    }

    
}
