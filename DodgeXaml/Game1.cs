using System;
using BloomPostprocessWin8;
using DodgeXaml.Levels;
using DodgeXaml.VisualEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Windows.UI.Xaml;
using Windows.Storage;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input.Touch;
using DodgeXaml.CommonHelper;
using GameComponents;
using X2DPE;

namespace DodgeXaml
{
    public class Game1 : Game
    {
        public static Game1 Current { get; set; }
        public bool NavigateFromStartPage = false;

        const string keyHighestScore = "KeyHighestScore";

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // ParticleText 
        SpriteFont font;
        public static Texture2D ParticleTextTexture;
        ParticleText particleText;
        SoundEffect LogoAudio;

        BloomComponent bloom;

        public Viewport GameViewPort
        {
            get
            {
                return GraphicsDevice.Viewport;
            }
        }

        public Vector2 ScreenSize
        {
            get 
            {
                return new Vector2(GameViewPort.Width, GameViewPort.Height);
            }
        }

        private SpriteManager spriteManager;
        public PowerUpManager powerUpsManager;
        private LevelControl levelCobtrol;

        public TouchInputManager touchInputManager;

        // SoundEffect stuff
        public SoundEffect soundEffectStart;
        public SoundEffect soundEffectTrack;

        private SoundEffectInstance soundEffectInstanceStart;
        private SoundEffectInstance soundEffectInstanceTrack;

        // Set offset to make string show in the middle of the screen
        private int offset = 35;

        // 全局随机数，伪随机数
        // Random number generator
        public Random rnd { get; private set; }

        // Score stuff
        public static int currentScore = 0;
        public static int previousScore = 0;
        public static int HighestScore { get; private set; }
        private const string highScoreFilename = "highscore.txt";
        private static int scoreForExtraLife = 2000;

        public SpriteFont scoreFont;
        private SpriteFont infoFont;

        // Background
        private Texture2D backgroundTexture;

        // Game states
        public enum GameState
        {
            Logo,
            Start,
            InGame,
            GameOver
        };
        
        public GameState CurrentGameState = GameState.Logo;

        // Lives remaining
        private static int numberLivesRemaining = 8;

        public int NumberLivesRemaining
        {
            get { return numberLivesRemaining; }
            set
            {
                numberLivesRemaining = value;

                if (numberLivesRemaining == 0)
                {
                    CurrentGameState = GameState.GameOver;
                    spriteManager.Enabled = false;
                    spriteManager.Visible = false;
                }
            }
        }


        MouseState preMouseState;
        MouseState curMouseState;

        FpsGameComponent fpsGameComponent;

        private ParticleEffectManager particleEffectManager;

        // 处理RainDrop
        public ParticleComponent particleComponent;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Current = this;

            rnd = new Random();

            // Extend battery life under lock.
            // The time to sleep when the game is inactive.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            TouchPanel.EnabledGestures = GestureType.Hold | GestureType.Tap | GestureType.DoubleTap | GestureType.Flick |
               GestureType.FreeDrag | GestureType.HorizontalDrag | GestureType.VerticalDrag;

        }

        public static void Reset()
        {
            //if (currentScore > HighScore)
            //{
            //    HighScore = currentScore;
                
            //}

            currentScore = 0;
            numberLivesRemaining = 8;
            scoreForExtraLife = 2000;
        }
        //#endregion

        protected override void Initialize()
        {
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            HighestScore = (int)FileHelper.GetData(keyHighestScore);


            spriteManager = new SpriteManager(this);
            Components.Add(spriteManager);
            spriteManager.Enabled = false;
            spriteManager.Visible = false;

            powerUpsManager = new PowerUpManager(this);
            Components.Add(powerUpsManager);
            powerUpsManager.Enabled = true;
            powerUpsManager.Visible = true;

            bloom = new BloomComponent(this);
            Components.Add(bloom);
            
            levelCobtrol = new LevelControl(this);
            Components.Add(levelCobtrol);

            touchInputManager = new TouchInputManager(this);
            Components.Add(touchInputManager);
            touchInputManager.Enabled = false;
            touchInputManager.Visible = false;

            fpsGameComponent = new FpsGameComponent(this, "debugFont");
            Components.Add(fpsGameComponent);

            // 处理RainDrop
            particleComponent = new ParticleComponent(this);
            Components.Add(particleComponent);
            

            particleEffectManager = new ParticleEffectManager(this);
            Components.Add(particleEffectManager);
            particleEffectManager.Enabled = false;
            particleEffectManager.Visible = false;

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>(@"Logo/Font");

            ParticleTextTexture = Content.Load<Texture2D>(@"Logo/Text Particle");
            LogoAudio = Content.Load<SoundEffect>(@"Audio/LogoAudio");
            
            particleText = new ParticleText(GraphicsDevice, font, "Farseer Game", ParticleTextTexture);
           
            // Load audio
            soundEffectStart = Content.Load<SoundEffect>(@"Audio/start");
            soundEffectTrack = Content.Load<SoundEffect>(@"Audio/track");

            // Play audio
            soundEffectInstanceStart = soundEffectStart.CreateInstance();
            //soundEffectInstanceStart.IsLooped = true;

            soundEffectInstanceTrack = soundEffectTrack.CreateInstance();
            soundEffectInstanceTrack.IsLooped = true;

            // Load fonts
            scoreFont = Content.Load<SpriteFont>(@"Fonts\Score");
            infoFont = Content.Load<SpriteFont>(@"Fonts\Info");

            // Load the background
            backgroundTexture = Content.Load<Texture2D>(@"Images\spooky_forest");

        }

        public void PauseAudio()
        {

            if (soundEffectInstanceStart.State == SoundState.Playing)
            {
                soundEffectInstanceStart.Pause();
            }
            if (soundEffectInstanceTrack.State == SoundState.Playing)
            {
                soundEffectInstanceTrack.Pause();
            }
        }

        public void PlayAudio()
        {

            if (soundEffectInstanceTrack.State != SoundState.Playing)
            {
                if (soundEffectInstanceTrack.State == SoundState.Paused)
                {
                    soundEffectInstanceTrack.Resume();
                }
                if (soundEffectInstanceTrack.State == SoundState.Stopped)
                {
                    soundEffectInstanceTrack.Play();
                }
            }

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        bool temp = true;

        protected override void Update(GameTime gameTime)
        {

            UpdateFromStartPage();

            SetVisibility(GamePage.Current.ellipseStore, 
                          GamePage.Current.ellipseLighting);

            // 根据目前的游戏状态执行相应的操作
            switch (CurrentGameState)
            {
                case GameState.Logo:
                    particleText.Update();
                    
                    if (temp)
                    {
                        LogoAudio.Play();
                        temp = false;
                    }

                    bool taped = false;
                    TouchCollection coll = TouchPanel.GetState();
                    int count = coll.Count;
                    if (count >= 1)
                    {
                        taped = coll[0].State == TouchLocationState.Released;
                    }
                          
                    if (particleText.LogoOver && 
                        (Keyboard.GetState().GetPressedKeys().Length > 0 
                        || taped))
                    {
                        CurrentGameState = GameState.Start;
                    }
                    break;

                case GameState.Start:
               
                    // Set the current window back to the game
                    if (Windows.UI.Xaml.Window.Current.Content is GamePage && !NavigateFromStartPage)
                    {
                        Windows.UI.Xaml.Window.Current.Content = new StartPage();
                    }
                    if (StartPage.Current.StartGameButtonClicked)
                    {
                        Windows.UI.Xaml.Window.Current.Content = GamePage.Current;
                        CurrentGameState = GameState.InGame;
                        //PlayAudio();
                        spriteManager.Enabled = true;
                        spriteManager.Visible = true;
                        StartPage.Current.StartGameButtonClicked = false;
                    }

                    break;
                case GameState.InGame:
                   
                    IsMouseVisible = true;
                    touchInputManager.Enabled = true;
                    touchInputManager.Visible = true;

                    particleEffectManager.Enabled = true;
                    particleEffectManager.Visible = true;

                    break;
                case GameState.GameOver:
                    FileHelper.SaveData(keyHighestScore, HighestScore);

                    touchInputManager.Enabled = false;
                    touchInputManager.Visible = false;
                    
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        //spriteManager.Enabled = false;
                        //spriteManager.Visible = false;
                        CurrentGameState = GameState.Start;
                        Reset();
                        
                    }

                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateFromStartPage()
        {
            if (StartPage.Current == null)
            {
                return;
            }
            bool isAudioOn = StartPage.Current.ToggleSwitchAudioIsOn;
            bool hasToggled = StartPage.Current.HasToggled;
            if (hasToggled)
            {
                if (isAudioOn)
                {
                    PlayAudio();
                }
                else
                {
                    PauseAudio();
                }
            }
            StartPage.Current.HasToggled = false;

            bool isShowFPS = StartPage.Current.ToggleSwitchShowFPSIsOn;
            bool hasFPSToggled = StartPage.Current.HasFPSToggled;
            if (hasFPSToggled)
            {
                if (isShowFPS)
                {
                    fpsGameComponent.Enabled = fpsGameComponent.Visible = true;
                }
                else
                {
                    fpsGameComponent.Enabled = fpsGameComponent.Visible = false;
                }
            }
            StartPage.Current.HasFPSToggled = false;

            fpsGameComponent.Enabled 
                = fpsGameComponent.Visible 
                = StartPage.Current.ToggleSwitchShowFPSIsOn;
        }

        protected override void Draw(GameTime gameTime)
        {
            bloom.BeginDraw();
            
            // 根据当前游戏状态进行绘制
            switch (CurrentGameState)
            {
                case GameState.Logo:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin(0, BlendState.Additive);
			        particleText.Draw(spriteBatch);
                    if (particleText.LogoOver)
                    {
                        string text = "Press any key or tap to continue!";
                        Vector2 position = new Vector2(ScreenSize.X / 2 - font.MeasureString(text).X * 0.6f / 2, 150);
                        spriteBatch.DrawString(font, text, position, Color.AliceBlue, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 1);
                    }
			        
                    spriteBatch.End();
                    break;

                case GameState.Start:
                    break;

                case GameState.InGame:
                    GraphicsDevice.Clear(Color.AliceBlue);

                    spriteBatch.Begin();

                    // Draw background image
                    spriteBatch.Draw(backgroundTexture,
                        new Rectangle(0, 0, Window.ClientBounds.Width,
                            Window.ClientBounds.Height), null,
                        Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0);

                    // Draw fonts
                    spriteBatch.DrawString(scoreFont,
                        "Score: " + currentScore,
                        new Vector2(50, 10), Color.DarkBlue,
                        0, Vector2.Zero,
                        1, SpriteEffects.None, 1);

                    spriteBatch.DrawString(scoreFont,
                       "Highest Score: " + HighestScore,
                       new Vector2(200, 10), Color.DarkBlue,
                       0, Vector2.Zero,
                       1, SpriteEffects.None, 1);

                    spriteBatch.End();
                    break;

                case GameState.GameOver:
                    GraphicsDevice.Clear(Color.AliceBlue);

                    spriteBatch.Begin();
                    string gameover = "Game Over! The blades win again!";
                    spriteBatch.DrawString(infoFont, gameover,
                        new Vector2((Window.ClientBounds.Width / 2f)
                                    - (infoFont.MeasureString(gameover).X / 2f),
                            (Window.ClientBounds.Height / 2f)
                            - (infoFont.MeasureString(gameover).Y / 2f)
                            - 2*offset),
                        Color.SaddleBrown);

                    gameover = "Your score: " + currentScore + "  Highest Score: " + HighestScore;
                    spriteBatch.DrawString(infoFont, gameover,
                        new Vector2((Window.ClientBounds.Width / 2f)
                                    - (infoFont.MeasureString(gameover).X / 2f),
                            (Window.ClientBounds.Height / 2f)
                            - (infoFont.MeasureString(gameover).Y / 2f)
                            ),
                        Color.SaddleBrown);

                    gameover = "(Press ENTER to play again!)";
                    spriteBatch.DrawString(infoFont, gameover,
                        new Vector2((Window.ClientBounds.Width / 2f)
                                    - (infoFont.MeasureString(gameover).X / 2f),
                            (Window.ClientBounds.Height / 2f)
                            - (infoFont.MeasureString(gameover).Y / 2f)
                            + 2*offset),
                        Color.SaddleBrown);

                    spriteBatch.End();
                    break;
            }

            base.Draw(gameTime);
        }

        public void PlaySound(string soundName)
        {
            // Load audio
            soundEffectStart = Content.Load<SoundEffect>(@"Audio/" + soundName);

            // Play audio
            SoundEffectInstance soundEffectInstance = soundEffectStart.CreateInstance();
            soundEffectInstance.Play();
        }

        public void AddScore(int score, GameTime gameTime)
        {
            previousScore = currentScore;
            currentScore += score;
            
            while (currentScore >= scoreForExtraLife)
            {
                scoreForExtraLife += 2000;
                NumberLivesRemaining++;
            }

            if (currentScore > HighestScore)
            {
                HighestScore = currentScore;

                
                //if (gameTime.TotalGameTime.Seconds % 60 == 0)
                //{
                    FileHelper.SaveData(keyHighestScore, HighestScore);
                //}
            }

        }

        private void SetVisibility(params UIElement[] uiElements)
        {
            if (CurrentGameState == GameState.InGame)
            {
                foreach (UIElement uiElement in uiElements)
                {
                    uiElement.Visibility = Visibility.Visible;
                }
            }
            else
            {
                foreach (UIElement uiElement in uiElements)
                {
                    uiElement.Visibility = Visibility.Collapsed;
                }
            }
            
        }
    }
}
