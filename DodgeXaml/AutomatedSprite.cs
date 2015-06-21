using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DodgeXaml
{
    public class AutomatedSprite : Sprite
    {
        // Sprite is automated. Direction is same as speed
        public override Vector2 direction
        {
            get { return speed; }
        }

        public AutomatedSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, string collisionCueName, int scoreValue, int health)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, collisionCueName, scoreValue, health)
        {
        }

        public AutomatedSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, int millisecondsPerFrame, string collisionCueName,
            int scoreValue, int health)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame, collisionCueName, scoreValue, health)
        {
        }

        public AutomatedSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, string collisionCueName, int scoreValue, int health, float scale)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, collisionCueName, scoreValue, health, scale)
        {
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // Move sprite based on direction
            position += direction;

            base.Update(gameTime, clientBounds);
        }
    }
}
