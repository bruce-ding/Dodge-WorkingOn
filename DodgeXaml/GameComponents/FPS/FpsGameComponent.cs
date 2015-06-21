using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameComponents
{
    /// <summary>
    /// A reusable component for tracking the frame rate.
    /// </summary>
    public class FpsGameComponent : DrawableGameComponent
    {
        #region Fields

        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        string fontName;

        Vector2 fpsPos;
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        #endregion

        #region Initialization

        public FpsGameComponent(Game game, string fullFontName)
            : base(game)
        {
            content = game.Content;
            fontName = fullFontName;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = content.Load<SpriteFont>(fontName);
        }

        protected override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = string.Format("{0} FPS", frameRate);

            spriteBatch.Begin();

            fpsPos = new Vector2((GraphicsDevice.Viewport.Width - spriteFont.MeasureString(fps).X) - 15, 10);
            spriteBatch.DrawString(spriteFont, fps, fpsPos, Color.White);

            spriteBatch.End();
        }

        #endregion
    }
}