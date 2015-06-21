using DodgeXaml.CommonHelper;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace DodgeXaml.Levels
{
    public class LevelControl : DrawableGameComponent
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState currentKeyboardState, previousKeyboardState;
        MouseState currentMouseState, previousMouseState;

        TouchCollection touches, previousTouches;

        private bool showSelection = false;
        // 暂停
        private SpriteFont pauseGameFont;
        // 重新开始
        private Texture2D newGameTexture2D;
        // 退出游戏
        private Texture2D exitGameTexture2D;
        // 回到游戏
        private Texture2D resumeGameTexture2D;

        private LinearAnimation animation1;
        private LinearAnimation animation2;
        private LinearAnimation animation3;

        private float newGameTextureWidth;
        private float resumeGameTextureWidth;
        private float exitGameTextureWidth;

        private bool shouldResetPosition = false;

        private Vector2 pauseTextPosition;
       

        public LevelControl(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            
        }
       
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pauseGameFont = Game1.Current.Content.Load<SpriteFont>(@"Fonts/Info");
            newGameTexture2D = Game1.Current.Content.Load<Texture2D>(@"Images/NewGame");
            resumeGameTexture2D = Game1.Current.Content.Load<Texture2D>(@"Images/ResumeGame");
            exitGameTexture2D = Game1.Current.Content.Load<Texture2D>(@"Images/ExitGame");

            pauseTextPosition = new Vector2((Game1.Current.ScreenSize.X- pauseGameFont.MeasureString("Paused").X )/ 2, 10);

            newGameTextureWidth = newGameTexture2D.Width;
            resumeGameTextureWidth = resumeGameTexture2D.Width;
            exitGameTextureWidth = exitGameTexture2D.Width;

            animation1 = new LinearAnimation();
            //animation1.FromX = -newGameTextureWidth;
            //animation1.ToX = (float)GraphicsDevice.Viewport.Width / 2 - newGameTextureWidth / 2;
            animation1.FromY = (float)GraphicsDevice.Viewport.Height / 2;
            animation1.ToY = (float)GraphicsDevice.Viewport.Height / 2;
            animation1.XDuration = TimeSpan.FromSeconds(1);
            animation1.YDuration = TimeSpan.FromSeconds(1);
            animation1.AlphaDuration = TimeSpan.FromSeconds(1);
            animation1.FromAlpha = 0;
            animation1.ToAlpha = 1.0f;
            animation1.ShowBeforeAnimation = true;
            animation1.TargeTexture2D = newGameTexture2D;

            animation2 = new LinearAnimation();
            //animation2.FromX = (float)GraphicsDevice.Viewport.Width + resumeGameTextureWidth;
            //animation2.ToX = (float)GraphicsDevice.Viewport.Width / 2 - resumeGameTextureWidth / 2;
            animation2.FromY = (float)GraphicsDevice.Viewport.Height / 2 + 50;
            animation2.ToY = (float)GraphicsDevice.Viewport.Height / 2 + 50;
            animation2.XDuration = TimeSpan.FromSeconds(1);
            animation2.YDuration = TimeSpan.FromSeconds(1);
            animation2.AlphaDuration = TimeSpan.FromSeconds(1);
            animation2.FromAlpha = 0;
            animation2.ToAlpha = 1.0f;
            animation2.ShowBeforeAnimation = true;
            animation2.TargeTexture2D = resumeGameTexture2D;


            animation3 = new LinearAnimation();
            //animation3.FromX = (float)GraphicsDevice.Viewport.Width + exitGameTextureWidth;
            //animation3.ToX = (float)GraphicsDevice.Viewport.Width / 2 - exitGameTextureWidth / 2;
            animation3.FromY = (float)GraphicsDevice.Viewport.Height / 2 + 100;
            animation3.ToY = (float)GraphicsDevice.Viewport.Height / 2 + 100;
            animation3.XDuration = TimeSpan.FromSeconds(1);
            animation3.YDuration = TimeSpan.FromSeconds(1);
            animation3.AlphaDuration = TimeSpan.FromSeconds(1);
            animation3.FromAlpha = 0;
            animation3.ToAlpha = 1.0f;
            animation3.ShowBeforeAnimation = true;
            animation3.TargeTexture2D = exitGameTexture2D;

            Reset();

            base.LoadContent();
        }

        
        private Rectangle BoundsToRectangle(int x, int y, Rectangle bounds)
        {
            return new Rectangle(x, y, bounds.Width, bounds.Height);
        }

        private Rectangle BoundsToRectangle(Vector2 position, Rectangle bounds)
        {
            return new Rectangle((int)position.X, (int)position.Y, bounds.Width, bounds.Height);
        }

        private void Reset()
        {
            animation1.FromX = - newGameTextureWidth;
            animation1.ToX = (float)GraphicsDevice.Viewport.Width / 2 - newGameTextureWidth / 2;

            animation2.FromX = GraphicsDevice.Viewport.Width + resumeGameTextureWidth;
            animation2.ToX = (float)GraphicsDevice.Viewport.Width / 2 - resumeGameTextureWidth / 2;

            animation3.FromX = GraphicsDevice.Viewport.Width + exitGameTextureWidth;
            animation3.ToX = GraphicsDevice.Viewport.Width / 2f - exitGameTextureWidth / 2;
        }

        private void UpdateAnimation(GameTime gameTime)
        {

            if (!animation1.IsAllCompleted)
            {
                animation1.Update(gameTime);

            }
            if (!animation2.IsAllCompleted)
            {
                animation2.Update(gameTime);
            }

            if (!animation3.IsAllCompleted)
            {
                animation3.Update(gameTime);
            }

        }

        public override void Update(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            
            // TODO: Add your update logic here
            if (Game1.Current.CurrentGameState == Game1.GameState.InGame)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape))
                {
                    showSelection = !showSelection;
                    if (showSelection)
                    {
                        SpriteManager.Current.Enabled = false;
                        Reset();
                    }
                    else
                    {
                        SpriteManager.Current.Enabled = true;
                        
                    }
                }

                if (showSelection)
                {
                    UpdateAnimation(gameTime);
                }
                

                if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                {
                    if (BoundsToRectangle(animation1.CurrentPosition, newGameTexture2D.Bounds).Contains(currentMouseState.Position))
                    {
                        showSelection = false;
                        
                    }
                    // 判断是否点击了ResumeGameTexture
                    else if (BoundsToRectangle(animation2.CurrentPosition, resumeGameTexture2D.Bounds).Contains(currentMouseState.Position))
                    {
                        showSelection = false;
                        
                        SpriteManager.Current.Enabled = true;
                    }
                   
                    else if (BoundsToRectangle(animation3.CurrentPosition, exitGameTexture2D.Bounds).Contains(new Point(currentMouseState.X, currentMouseState.Y)))
                    {
                        showSelection = false;
                        
                    }
                }
            }
            
            
            base.Update(gameTime);
        }

       
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            if (showSelection)
            {
                spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
                spriteBatch.DrawString(pauseGameFont, "Paused", new Vector2(600, 50),Color.Red);

                if (animation1.ShowBeforeAnimation)
                {
                    animation1.Draw(spriteBatch);
                }
                if (animation2.ShowBeforeAnimation)
                {
                    animation2.Draw(spriteBatch);
                }
                if (animation3.ShowBeforeAnimation)
                {
                    animation3.Draw(spriteBatch);
                }
                spriteBatch.End();

                
            }

           base.Draw(gameTime);
           
        }
        
   }
}
