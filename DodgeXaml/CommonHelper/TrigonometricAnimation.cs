using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DodgeXaml.CommonHelper
{
    public class TrigonometricAnimation
    {
        // The gem is animated from a base position along the Y axis.
        public Vector2 basePosition;
        public float bounce;
        public Texture2D texture;
        public Vector2 origin;

        public readonly Color Color = Color.Red;

        // Bounce control constants
        public float BounceHeight = 0.58f;
        public float BounceRate = 3.0f;
        public float BounceSync = -0.75f;

        /// <summary>
        /// Gets the current position of this gem in world space.
        /// </summary>
        private Vector2 Position
        {
            get
            {
                return basePosition + new Vector2(0.0f, bounce);
            }
        }
        public void Update(GameTime gameTime)
        {
            

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);

        }

    }
}
