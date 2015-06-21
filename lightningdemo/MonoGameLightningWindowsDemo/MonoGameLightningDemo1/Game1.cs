#region Using Statements
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using MonoGameLightningDemoLib;
#endregion

namespace MonoGameLightningDemo1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        enum Mode { SimpleLightning, BranchLightning, LightningText }
        Mode mode;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont lightningFont, infoFont;

        KeyboardState keyState, lastKeyState;
        MouseState mouseState, lastMouseState;
        List<ILightning> bolts = new List<ILightning>();
        LightningText lightningText;
        RenderTarget2D lastFrame, currentFrame;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            lightningFont = Content.Load<SpriteFont>("LightningFont");
            infoFont = Content.Load<SpriteFont>("InfoFont");

            Point screenSize = new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            lastFrame = new RenderTarget2D(GraphicsDevice, screenSize.X, screenSize.Y, false, SurfaceFormat.Color, DepthFormat.None);
            currentFrame = new RenderTarget2D(GraphicsDevice, screenSize.X, screenSize.Y, false, SurfaceFormat.Color, DepthFormat.None);

            // Initialize lastFrame to be solid black
            GraphicsDevice.SetRenderTarget(lastFrame);
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.SetRenderTarget(null);

            Art.Load(Content);
            lightningText = new LightningText(GraphicsDevice, spriteBatch, lightningFont, "Lightning");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            lastKeyState = keyState;
            keyState = Keyboard.GetState();
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            if (WasPressed(Keys.Space))
                mode = (Mode)(((int)mode + 1) % 3);

            var screenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);

            switch (mode)
            {
                case Mode.SimpleLightning:
                    if (WasClicked())
                    {
                        bolts.Add(new LightningBolt(screenSize/2, mousePosition));
                        bolts.Add(new LightningBolt(screenSize/2, mousePosition));
                        bolts.Add(new LightningBolt(screenSize / 2, mousePosition));

                    }

                    break;
                case Mode.BranchLightning:
                    if (WasClicked())
                        bolts.Add(new BranchLightning(screenSize / 2, mousePosition));

                    break;
                case Mode.LightningText:
                    lightningText.Update();
                    break;
            }

            foreach (var bolt in bolts)
                bolt.Update();

            bolts = bolts.Where(x => !x.IsComplete).ToList();

            base.Update(gameTime);
        }

        // return true if a key was pressed down this frame
        bool WasPressed(Keys key)
        {
            return keyState.IsKeyDown(key) && lastKeyState.IsKeyUp(key);
        }

        // return true if the left mouse button was clicked down this frame
        bool WasClicked()
        {
            return mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released;
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            // The lightning text is drawn a bit differently due to our optimization with the render targets.
            if (mode == Mode.LightningText)
                DrawLightningText();
            else
                DrawLightning();

            spriteBatch.Begin();
            spriteBatch.DrawString(infoFont, "" + mode, new Vector2(5), Color.White);
            spriteBatch.DrawString(infoFont, "Press space to change mode", new Vector2(5, 30), Color.White);

            if (mode != Mode.LightningText)
                spriteBatch.DrawString(infoFont, "Click to make lightning", new Vector2(5, 55), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void DrawLightningText()
        {
            GraphicsDevice.SetRenderTarget(currentFrame);
            GraphicsDevice.Clear(Color.Black);

            // draw our last frame at 96% of its original brightness
            spriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp, null, null);
            spriteBatch.Draw(lastFrame, Vector2.Zero, Color.White * 0.96f);
            spriteBatch.End();

            // draw the new lightning bolts
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            lightningText.Draw();
            spriteBatch.End();

            // draw currentFrame to the backbuffer
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp, null, null);
            spriteBatch.Draw(currentFrame, Vector2.Zero, Color.White);
            spriteBatch.End();

            Swap(ref currentFrame, ref lastFrame);
        }

        void DrawLightning()
        {
            GraphicsDevice.Clear(Color.Black);

            // we use SpriteSortMode.Texture to improve performance
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);

            foreach (var bolt in bolts)
                bolt.Draw(spriteBatch);

            spriteBatch.End();
        }

        void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

    }
}
