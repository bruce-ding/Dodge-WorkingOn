using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using DodgeXaml.Frineds;
using DodgeXaml.ScreenInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Supernova.Particles2D;
using DodgeXaml.Utilities;
using Supernova.Particles2D.Modifiers.Alpha;
using Supernova.Particles2D.Modifiers.Movement.Gravity;

namespace DodgeXaml
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TouchInputManager : DrawableGameComponent
    {
       
        public static TouchInputManager Current { get; set; }

        SpriteBatch spriteBatch;

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
       public ScreenPadState currentOSCState;

        public TouchInputManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            Current = this;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            screenPad = new ScreenPad
            (
                Game1.Current,
                Game1.Current.Content.Load<Texture2D>("GFX/ThumbBase"),
                Game1.Current.Content.Load<Texture2D>("GFX/ThumbStick"),
                Game1.Current.Content.Load<Texture2D>("GFX/Dpad_All"),
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

            font = Game1.Current.Content.Load<SpriteFont>("debugFont");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            screenPad.Update();

            currentOSCState = screenPad.GetState();
            Vector2 pos = SpriteManager.Current.player.CenterPosition;
            pos += currentOSCState.ThumbSticks.Left;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
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
