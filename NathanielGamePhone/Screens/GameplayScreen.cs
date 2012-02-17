using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using EasyStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using NathanielGame.Levels;
using NathanielGame.Utility;

namespace NathanielGame
{
    public enum GameToPlay
    {
        NewGame,
        LoadGame,
        SurvivalGame,
    }
    class GameplayScreen : GameScreen
    {
        #region Fields
        private Viewport _vp;
        public Viewport VP
        {
            get { return _vp; }
        }
        private ContentManager _content;
        private static Camera _camera;
        public static Camera Camera
        {
            get { return _camera; }
        }
        private static double _elapsedTime;
        public double ElapsedTime
        {
            get { return _elapsedTime; }
        }
        private static double _totalTime;
        private static Vector2 _currentTouchPosition;
        public static GameCharacter FocusedAgent { get; set; }
        private static Vector2 _cameraCenter;
        private static IAsyncSaveDevice _saveDevice;
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(SaveGameData));
        private bool _doLoad;
        public static GameToPlay CurrentGameToPlay { get; private set; }
        private GameTime _gameTime;
        public ScreenManager GlobalScreenManager
        {
            get { return ScreenManager; }
        }

        public static bool IsPaused { get; set; }
        #endregion

        #region Screens

        private static PauseMenu _pauseMenu;
        public static PauseMenu GamePauseMenu
        {
            get
            {
                return _pauseMenu;
            }
        }
        private static GameOverScreen _gameOverScreen;
        private static WinScreen _winScreen;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(GameToPlay action)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            EasyStorageSettings.SetSupportedLanguages(Language.English);
            _saveDevice = new IsolatedStorageSaveDevice();
            SaveGameGlobal.SaveDevice = _saveDevice;
            _saveDevice.SaveCompleted += SaveDeviceSaveCompleted;
            //This will be used for drag and drop operations
            EnabledGestures = GestureType.FreeDrag |
                GestureType.DragComplete | GestureType.Tap;
            CurrentGameToPlay = action;
            if (action == GameToPlay.NewGame)
            {
                Player.currentLevel = 1;
            }
            else if (action == GameToPlay.SurvivalGame)
            {
                Player.currentLevel = 0;
            }
            else
            {
                if (Player.currentLevel > 1)
                    _doLoad = true;
                else
                    _doLoad = false;
            }

        }



        /// <summary>
        /// Load up all game elements
        /// </summary>
        public override void LoadContent()
        {
            try
            {
                FrameRateCounter.Initialize();
                if (_content == null)
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");
                ImageManager.LoadContent(_content);
                AnimationManager.Initialize();
                AnimationManager.LoadContent(_content);
                

                _vp = ScreenManager.GraphicsDevice.Viewport;
                IsPaused = false;
                if (_doLoad)
                {
                    LoadGame();
                    _doLoad = false;
                }
                GameManager.LoadCurrentLevel(this, Player.currentLevel, _content);
                SetupGame();
                SetUpHUD();
                _pauseMenu = new PauseMenu(ScreenManager.Game, this);
                _pauseMenu.Initialize();
                _gameOverScreen = new GameOverScreen(ScreenManager.Game, this);
                _gameOverScreen.Initialize();
                _winScreen = new WinScreen(ScreenManager.Game, this);
                _winScreen.Initialize();
                Audio.LoadSounds(_content);
                Audio.PlayMusic(Audio.GamePlayMusic);
                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
            catch
            {
                //Empty
            }
        }

        #endregion

        #region Input

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            try
            {
                if (input == null)
                    throw new ArgumentNullException("input");
                if (input.IsPauseGame(null))
                {
                    PauseCurrentGame();
                }
                // Look up inputs for the active player profile.
                if (ControllingPlayer != null)
                {
                    var playerIndex = (int)ControllingPlayer.Value;

#pragma warning disable 168
                    KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
                    GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];
#pragma warning restore 168
                }

                // if the user pressed the back button, we return to the main menu
                PlayerIndex player;
                if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
                {
                    //SaveGame();
                    LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(),
                                       new MainMenuScreen());

                }
                else
                {
                    foreach (GestureSample gest in input.Gestures)
                    {
                        //Screen touch point
                        var stp = new Point((int)gest.Position.X, (int)gest.Position.Y);
                        //Map touch point
                        var mtp = new Point((int)(_camera.Position.X + gest.Position.X),
                                            (int)(_camera.Position.Y + gest.Position.Y));

                        switch (gest.GestureType)
                        {
                            case GestureType.Tap:
                                {
                                    _currentTouchPosition = (_camera.Position + gest.Position);
                                    if (
                                        !(HUD.HandleInput(stp)
                                        || PlayerManager.HandleInput(mtp, stp)
                                        || EnemyManager.HandleInput(mtp))
                                        )
                                    {
                                        if (!IsPaused)
                                            PlayerManager.Nathaniel.Destination = (_currentTouchPosition);
                                    }

                                    if (IsPaused && !GameManager.GameOver)
                                    {
                                        _pauseMenu.HandleInput(stp);
                                    }
                                    if (GameManager.GameOver)
                                        _gameOverScreen.HandleInput(stp);
                                    if (GameManager.LevelWon)
                                        _winScreen.HandleInput(stp);
                                    //Touching the screen should move the main character unless an action is being performed
                                    //on something else
                                    

                                }
                                break;

                            case GestureType.FreeDrag:
                                {
                                    BuildStructureMenu.HandleBuildMenuSelection(stp, gest);
                                }
                                break;
                            case GestureType.DragComplete:
                                {
                                    BuildStructureMenu.TryPlaceDefensiveStructure();
                                }
                                break;
                        }
                    }
                }
            }
            catch
            {
                //Empty
            }
        }
        public void BackToMenu()
        {
            LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen());
        }
        public void ToHighScoresScreen()
        {
            ScreenManager.AddScreen(new HighScoresScreen(), ControllingPlayer);
        }
        public void ToNextLevel()
        {
            Player.currentLevel++;
            SaveGame();
            LoadingScreen.Load(ScreenManager, true, ControllingPlayer,
                               new GameplayScreen(GameToPlay.LoadGame));
        }
        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            FrameRateCounter.Update(gameTime);
            //OutPutPerformance();

            if (!IsPaused)
            {
                if (!IsActive)
                {
                    base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                    return;
                }
                _elapsedTime = gameTime.ElapsedGameTime.TotalSeconds;
                _totalTime += _elapsedTime;
                _gameTime = gameTime;
                AnimationManager.Update(gameTime);
                HUD.Update(gameTime);
                GameManager.UpdateCurrentLevel(gameTime);

                // Set camera target to player location (minus half the screen width and height to center on the player)
                if (_camera != null)
                {
                    _camera.Target = FocusedAgent.Position - _cameraCenter;
                    // Update the camera
                    _camera.Update();
                }
            }
            else
            {
                if (!GameManager.GameOver)
                    _pauseMenu.Update(gameTime);
            }

            if (GameManager.GameOver)
            {
                //TODO: Better implementation through popup screen
                PauseCurrentGame();
                _gameOverScreen.Update(gameTime);
            }
            if (GameManager.LevelWon)
            {
                PauseCurrentGame();
                _winScreen.Update(gameTime);
            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            FrameRateCounter.Draw(gameTime);
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                                 Color.Black * 0f, 0, 0);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            GameManager.DrawCurrentLevel(gameTime, spriteBatch);
            spriteBatch.Begin();

            HUD.Draw(spriteBatch, ScreenManager.Font, FocusedAgent, _totalTime);
            
                PlayerManager.BuildMenu.DrawMenu(gameTime,spriteBatch);
            
            if (GameManager.GameOver)
            {
                _gameOverScreen.DrawMenu(gameTime, spriteBatch);
            }
            if (GameManager.LevelWon)
            {
                _winScreen.DrawMenu(gameTime, spriteBatch);
            }
            else
            {
                _pauseMenu.DrawMenu(spriteBatch);
            }
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
        }

        #endregion

        #region Setup methods

        private void SetupGame()
        {
            // Initialise camera using the map to define the boundaries
            _camera = new Camera(ScreenManager.GraphicsDevice.Viewport, Level.CurrentMap);
            _cameraCenter = new Vector2((float)_camera.Width / 2, (float)_camera.Height / 2);
        }

        private void SetUpHUD()
        {
            HUD.Initialize(this);
        }

        #endregion


        #region Management methods

        public void DrawMessage(string message, TimeSpan howLong)
        {
            if (_gameTime != null)
            {
                TimeSpan checkPoint = _gameTime.TotalGameTime;
                ScreenManager.SpriteBatch.Begin();
                while (_gameTime.TotalGameTime - checkPoint < howLong)
                    ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, message,
                                                         new Vector2(
                                                             _vp.Width / 2f -
                                                             (message.Length * ScreenManager.Font.MeasureString(message).X /
                                                              2),
                                                             _vp.Height / 2f), Color.White);

                ScreenManager.SpriteBatch.End();
            }

        }

        public void PauseCurrentGame()
        {
            if (!IsPaused)
            {
                IsPaused = true;
                LevelTime.Pause();
                Audio.PauseMusic();
            }
        }

        public void ResumeCurrentGame()
        {
            if (IsPaused)
            {
                IsPaused = false;
                LevelTime.Resume();
                Audio.ResumeMusic();
            }
        }

        public void SaveGame()
        {
            try
            {
                // make sure the device is ready
                if (_saveDevice.IsReady)
                {
                    SaveGameGlobal.SaveDevice.Save(SaveGameGlobal.containerName, SaveGameGlobal.saveFileName,
                                                   SerializeData);
                    Debug.WriteLine("Saving");
                }
            }
            catch
            {
                Debug.WriteLine("Problem saving");
            }
        }


        private void SaveDeviceSaveCompleted(object sender, FileActionCompletedEventArgs args)
        {
            DrawMessage("Save Complete", TimeSpan.FromSeconds(2.0));
        }


        private static void SerializeData(Stream stream)
        {
            SaveGameGlobal.saveData.currentLevel = Player.currentLevel;
            SaveGameGlobal.saveData.resources = Player.resources;
            SaveGameGlobal.saveData.lives = Player.lives;
            SaveGameGlobal.saveData.sound = Player.soundOff;

            _serializer.Serialize(stream, SaveGameGlobal.saveData);
        }

        public static void LoadGame()
        {
            if (SaveGameGlobal.SaveDevice != null &&
                SaveGameGlobal.SaveDevice.FileExists(SaveGameGlobal.containerName, SaveGameGlobal.saveFileName))
            {
                try
                {
                    SaveGameGlobal.SaveDevice.Load(SaveGameGlobal.containerName, SaveGameGlobal.saveFileName, DeserializeData);
                    Player.currentLevel = SaveGameGlobal.saveData.currentLevel;
                    Player.lives = SaveGameGlobal.saveData.lives;
                    Player.resources = SaveGameGlobal.saveData.resources;
                    Player.soundOff = SaveGameGlobal.saveData.sound;
                }
                catch
                {
                    Debug.WriteLine("Problem loading game");
                }

            }
        }

        private static void DeserializeData(Stream stream)
        {
            SaveGameGlobal.loadData = (SaveGameData)_serializer.Deserialize(stream);
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            var componentList = ScreenManager.Game.Components;

            for (int index = 0; index < componentList.Count; index++)
            {
                if (componentList[index] != this && componentList[index] != ScreenManager &&
                    !(componentList[index] is Audio))
                {
                    componentList.RemoveAt(index);
                    index--;
                }
            }
            _elapsedTime = 0;
            _totalTime = 0;
            GameManager.CleanUp();
            _content.Unload();
            Audio.StopMusic();
            base.UnloadContent();
        }

        #endregion
        /*
        private void OutPutPerformance()
        {
            var memuse = (long)DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage");
            var peakmem = (long)DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage");
            var maxmem = (long)DeviceExtendedProperties.GetValue("DeviceTotalMemory");

            memuse /= 1024 * 1024;
            peakmem /= 1024*1024;
            maxmem /= 1024 * 1024;

            Debug.WriteLine("Current memory use = "+memuse+" MB. Total memory available = "+maxmem+" MB Peak memeory usage = "+peakmem+" MB");
        }
        */

    }
}
