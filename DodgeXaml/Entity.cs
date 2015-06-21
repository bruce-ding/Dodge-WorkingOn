//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DodgeXaml
{
    abstract class Entity
    {
        
        protected Texture2D image;
        // The tint of the image. This will also allow us to change the transparency.
        protected Color color = Color.White;

        public Vector2 Position, Velocity;
        public float Orientation;
        public float Radius = 20;	// used for circular collision detection
        public bool IsExpired;		// true if the entity was destroyed and should be deleted.

        public Vector2 Size
        {
            get
            {
                return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
            }
        }

        // Gets the collision rect based on position, framesize and collision offset
        public Rectangle CollisionRect
        {
            get
            {
                return new Rectangle((int)(Position.X), (int)(Position.Y), image.Width, image.Height);
            }
        }

        public Vector2 CenterPosition
        {
            get
            {
                return new Vector2(Position.X + image.Width / 2, Position.Y + image.Height / 2);
            }
        }

        public abstract void Update();

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, 1f, 0, 0);
            spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, 1f, 0, 0);

        }
    }
}
