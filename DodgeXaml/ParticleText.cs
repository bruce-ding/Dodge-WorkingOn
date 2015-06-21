using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DodgeXaml
{
	class ParticleText
	{
		static Random rand = new Random();

		public Vector2 Size { get { return textSize * scale; } }
		public Color Color = new Color(10, 30, 255);  // almost pure blue

        public bool LogoOver { get; set; }
        private bool temp = true;

		List<Particle> textParticles = new List<Particle>();
		Texture2D particleTexture;
		Vector2 screenSize;
		Vector2 textSize;
		float scale;

		public ParticleText(GraphicsDevice graphicsDevice, SpriteFont font, string text, Texture2D particleTexture, float scale = 4f)
		{
			this.scale = scale;
			this.particleTexture = particleTexture;

			var viewport = graphicsDevice.Viewport;
			screenSize = new Vector2(viewport.Width, viewport.Height);
			textSize = font.MeasureString(text);
			Vector2 offset = (screenSize / scale - textSize) / 2f;

			var textPoints = GetParticlePositions(graphicsDevice, font, text);

			foreach (var point in textPoints)
			{
				var particle = new Particle()
				{
					Position = GetRandomParticlePosition(), 
					Destination = point + offset
				};

				textParticles.Add(particle);
			}
		}

		// We get the destinations of our particles by drawing our font to a render target and
		// reading back which pixels were set.
		List<Vector2> GetParticlePositions(GraphicsDevice device, SpriteFont font, string text)
		{
			Vector2 size = font.MeasureString(text) + new Vector2(0.5f);
			int width = (int)size.X;
			int height = (int)size.Y;

			// Create a temporary render target and draw the font on it.
            //RenderTarget2D target = new RenderTarget2D(device, width, height);
            //RenderTarget2D target = new RenderTarget2D(device, width, height);
            //device.RasterizerState = new RasterizerState() { CullMode = CullMode.None, MultiSampleAntiAlias = true }; 
            RenderTarget2D target = new RenderTarget2D(device, width,height,false, SurfaceFormat.Color, DepthFormat.None);
            
            
            device.SetRenderTarget(target);
			device.Clear(Color.Black);
           

			SpriteBatch spriteBatch = new SpriteBatch(device);
			spriteBatch.Begin();
			spriteBatch.DrawString(font, text, Vector2.Zero, Color.White);
			spriteBatch.End();

			device.SetRenderTarget(null);	// unset the render target

			// read back the pixels from the render target
			Color[] data = new Color[width * height];
			target.GetData<Color>(data);
			target.Dispose();

			// Return a list of points corresponding to pixels drawn by the font. The font size will affect the number of
			// points and the quality of the text.
			List<Vector2> points = new List<Vector2>();
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					// Add all points that are lighter than 50% grey. The text is white, but due to anti-aliasing pixels
					// on the border may be shades of grey.
					if (data[width * y + x].B > 128)	
						points.Add(new Vector2(x, y));
				}
			}

			return points;
		}

		Vector2 GetRandomParticlePosition()
		{
			// Initially place particles randomly in a circle of radius screenSize.Y, centered in the middle of the screen.
			float theta = (float)rand.NextDouble() * MathHelper.TwoPi;
			float r = rand.Next((int)screenSize.Y);
			
			Vector2 pos = new Vector2(r * (float)Math.Cos(theta) + screenSize.X / 2, r * (float)Math.Sin(theta) + screenSize.Y / 2);
			return pos / scale;
		}

		public void Update()
		{
            foreach (var particle in textParticles)
            {
                particle.Update();
            }

            LogoOver = textParticles.TrueForAll(particle =>
            {
                if (particle.IsStill)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
           
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (var particle in textParticles)
			{
				Vector2 pos = particle.Position * scale;
				Vector2 origin = new Vector2(particleTexture.Width, particleTexture.Height) / 2f;
				spriteBatch.Draw(particleTexture, pos, null, Color, 0f, origin, 1f, SpriteEffects.None, 0);
			}
		}

		public void Reset()
		{
			foreach (var particle in textParticles)
				particle.Position = GetRandomParticlePosition();
		}

		class Particle
		{
			public Vector2 Position { get; set; }
			public Vector2 Destination { get; set; }
            public bool IsStill { get; set; }
            private int TimeOfwiggling = 120;

			private Vector2 velocity;

			public void Update()
			{
				// The particles will head towards their destination. Once they reach it, they will wiggle around a bit.
				if (Vector2.DistanceSquared(Position, Destination) > 2f)
				{
					// Make the particles spiral towards their destination, slowing down as they get close
					velocity = (Destination - Position) / 60f;
					velocity = Vector2.Transform(velocity, Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(30)));
					
					// cap the maximum velocity
					if (velocity.LengthSquared() > 0.5f * 0.5f)
						velocity = Vector2.Normalize(velocity) * 0.5f;

					Position += velocity;
				}
				else
				{
					// add a random acceleration to cause wiggling.
					velocity += GetRandomVector2(0.025f);
					
					// cap the maximum velocity
					if (velocity.LengthSquared() > 0.125f * 0.125f)
						velocity = Vector2.Normalize(velocity) * 0.25f;

					Position += velocity;

                    //--TimeOfwiggling;
                    //if (TimeOfwiggling <= 0)
                    //{
                        IsStill = true;
                    //}
				}
			}

			Vector2 GetRandomVector2(float length)
			{
				return new Vector2((float)rand.NextDouble() - 0.5f, (float)rand.NextDouble() - 0.5f) * length;
			}
		}
	}
}
