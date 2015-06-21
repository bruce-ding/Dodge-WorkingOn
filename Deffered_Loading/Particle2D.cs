using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Deffered_Loading
{
	public class Particle2D
	{
		public Texture2D texture;
		public Color ObjectColor;

		public Vector2 Position;
		public Vector2 Velocity;
		Vector2 origin;

		public float rotation = 0f;

		float scaleFactor = 0.15f;
		float scale = 0.1f;
		DefferedLoadingSample game;
		public bool IsActive = false;

		public Particle2D(DefferedLoadingSample game, ref Texture2D texture, Vector2 position, Vector2 velocity, float rotation)
		{
			this.game = game;
			Position = position;
			Velocity = velocity;
			this.texture = texture;

			rotation = 0f;
			origin = new Vector2(texture.Width / 2, texture.Height / 2);
			this.ObjectColor = Color.White;
		}

		public void Reset(Vector2 position, Vector2 Velocity, Color color, float rotation)
		{
			this.Position = position;
			this.Velocity = Velocity;
			this.rotation = rotation;
			this.ObjectColor = color;
			IsActive = true;
		}

		public void Update(GameTime gameTime)
		{
			if (!IsActive)
				return;

			rotation += 0.5f;
			Position += Velocity;
			Velocity *= 0.96f;

			scale += scale *= scaleFactor;
			SetAlpha(ObjectColor.A - 5);

			if (ObjectColor.A <= 0 || IsOffScreen())
				IsActive = false;
		}

		private bool IsOffScreen()
		{
			if (Position.X > game.ScreenSize.X + texture.Width / 2 || 
				Position.Y > game.ScreenSize.Y + texture.Height / 2 || 
				Position.X < -texture.Width / 2 || Position.Y < -texture.Height / 2)
				return true;

			return false;
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (!IsActive || IsOffScreen())
				return;

			spriteBatch.Draw(texture, Position, texture.Bounds, ObjectColor, rotation, origin, scale, SpriteEffects.None, 1f);
		}

		protected void SetAlpha(int alpha)
		{
			// Keep in the range 0 to 255
			if (alpha < 0) alpha = 0;
			if (alpha > 255) alpha = 255;

			// Update the color
			Color c = ObjectColor;
			c.A = (byte)alpha;
			ObjectColor = c;
		}
	}
}
