using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using xInput = Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ScreenControlsSample
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static MainGame Current { get; set; }

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

        SpriteFont font;

        /// <summary>
        /// The actual OSC
        /// </summary>
        ScreenPad screenPad;

        /// <summary>
        /// the value that <b>screenPad.GetState()</b> returns
        /// </summary>
        ScreenPadState currentOSCState;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
            graphics.IsFullScreen = true;
            this.IsMouseVisible = true;
            graphics.ApplyChanges();
            Current = this;

            Content.RootDirectory = "Content";

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

            screenPad = new ScreenPad
            (
                this,
                Content.Load<Texture2D>("GFX/ThumbBase"),
                Content.Load<Texture2D>("GFX/ThumbStick"),
                Content.Load<Texture2D>("GFX/Dpad_All"),
                Color.Blue,
                Color.Red,
                Color.Green
            );

            //screenPad = new ScreenPad
            //(
            //    this,
            //    Content.Load<Texture2D>("GFX/ThumbBase"),
            //    Content.Load<Texture2D>("GFX/ThumbStick"),
            //    Content.Load<Texture2D>("GFX/ABXY_buttons")
            //);

            font = Content.Load<SpriteFont>("debugFont");
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
            // Allows the game to exit
            if (xInput.GamePad.GetState(PlayerIndex.One).Buttons.Back == xInput.ButtonState.Pressed)
                this.Exit();

            screenPad.Update();

            currentOSCState = screenPad.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Wheat);

            spriteBatch.Begin();
            screenPad.Draw(gameTime, spriteBatch);

            spriteBatch.DrawString(font, string.Format("Left Stick: {0}", currentOSCState.ThumbSticks.Left.ToString()), Vector2.Zero, Color.Black);

            if (screenPad.XTYPE)
            {
                spriteBatch.DrawString(font, string.Format("X: {0}", (currentOSCState.Buttons.X == ButtonState.Pressed).ToString()), new Vector2(0, 50), Color.Black);
                spriteBatch.DrawString(font, string.Format("Y: {0}", (currentOSCState.Buttons.Y == ButtonState.Pressed).ToString()), new Vector2(0, 80), Color.Black);
                spriteBatch.DrawString(font, string.Format("A: {0}", (currentOSCState.Buttons.A == ButtonState.Pressed).ToString()), new Vector2(0, 110), Color.Black);
                spriteBatch.DrawString(font, string.Format("B: {0}", (currentOSCState.Buttons.B == ButtonState.Pressed).ToString()), new Vector2(0, 140), Color.Black);
            }
            else
            {
                spriteBatch.DrawString(font, string.Format("Right Stick: {0}", currentOSCState.ThumbSticks.Right.ToString()), new Vector2(0, 50), Color.Black);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
