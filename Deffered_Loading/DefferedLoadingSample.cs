using System;
using System.Collections.Generic;
using System.Linq;
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
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class DefferedLoadingSample : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		bool loadingStarted = false;

		Vector2 logoPosition;
		Texture2D logo;
		float logoRotation = 0f;

		Vector2 textPos;

		float logoScale = 1f;
		Vector2 screenSize;

		double currentTime;
		bool readyToExplode = false;
		bool exploded = false;
		bool allParticlesDead = false;

		private Color logoColor;
		private SpriteFont font;
		string gameplayText = "Loaded. Show menu, or something elese";

		public Particle2D[] LogoScreenParticles;

		public Vector2 ScreenSize
		{
			get { return screenSize; }
		}

		GameState currentState = GameState.Logo;

		public GameState CurrentState
		{
			get { return currentState; }
			set { currentState = value; }
		}

		public DefferedLoadingSample()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			
			// Extend battery life under lock.
			InactiveSleepTime = TimeSpan.FromSeconds(1);
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
			graphics.IsFullScreen = true;
			graphics.ApplyChanges();

			base.Initialize();
		}

		private void ContinueLoading(GameTime gameTime)
		{
			loadingStarted = true;
			
			LogoScreenParticles = new Particle2D[50];
			Texture2D particleTexture = Content.Load<Texture2D>("flare");
			font = Content.Load<SpriteFont>("Arial");

			Vector2 size = font.MeasureString(gameplayText);
			textPos = new Vector2((screenSize.X - size.X) / 2, (screenSize.Y - size.Y) / 2);

			for (int i = 0; i < LogoScreenParticles.Length; i++)
			{
				Particle2D particle = new Particle2D(this, ref particleTexture, screenSize / 2f, Vector2.Zero, 0f);
				LogoScreenParticles[i] = particle;
			}

			readyToExplode = true;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			logo = Content.Load<Texture2D>("book");
			logoColor = Color.White;
			logoColor.A = 0;
			screenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
			logoPosition = screenSize / 2f;
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
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				Exit();

			if (!loadingStarted)
				ContinueLoading(gameTime);

			switch (currentState)
			{
				case GameState.Logo:
					UpdateLogo(gameTime);
					break;
				case GameState.Gameplay:

					break;
				case GameState.Loading:
					break;
			}

			base.Update(gameTime);
		}

		private void UpdateLogo(GameTime gameTime)
		{
			if ((gameTime.TotalGameTime.TotalSeconds - currentTime) > 0.05)
			{
				//logoRotation = GameHelper.RandomNext(-0.1f, 0.1f);
				//logoScale += GameHelper.RandomNext(-0.02f, 0.03f);
				currentTime = gameTime.TotalGameTime.TotalSeconds;
			}

			SetAlpha(logoColor.A + 1);


			
			if (gameTime.TotalGameTime.TotalSeconds > 4)
			{
				if (readyToExplode && !exploded)
				{
					for (int i = 0; i < LogoScreenParticles.Length; i++)
					{
						Vector2 position = logoPosition + new Vector2(GameHelper.RandomNext(-logo.Width / 2, logo.Width / 2), GameHelper.RandomNext(-logo.Height / 2, logo.Height / 2));
						Vector2 velocity = new Vector2(GameHelper.RandomNext(-80f, 80f), GameHelper.RandomNext(-10f, 10f));
						int clr = GameHelper.RandomNext(1, 2);
						Color color = new Color(GameHelper.RandomNext(128, 255), GameHelper.RandomNext(128, 255), GameHelper.RandomNext(128, 255));
						
						LogoScreenParticles[i].Reset(position, velocity, color, GameHelper.RandomNext(0, MathHelper.ToRadians(360)));
					}

					exploded = true;
				}
			}

			if (exploded)
			{
				allParticlesDead = true;
				for (int i = 0; i < LogoScreenParticles.Length; i++)
				{
					if (LogoScreenParticles[i].IsActive)
					{
						allParticlesDead = false;
					}
				}

				if (allParticlesDead)
				{
					currentState = GameState.Gameplay;
				}
			}

			for (int i = 0; i < LogoScreenParticles.Length; i++)
			{
				LogoScreenParticles[i].Update(gameTime);
			}

		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			switch (currentState)
			{
				case GameState.Logo:
					GraphicsDevice.Clear(Color.Black);
					DrawLogo(gameTime, spriteBatch);
					break;
				case GameState.Gameplay:
					GraphicsDevice.Clear(Color.Black);
					DrawGamePlay(gameTime);
					break;
				case GameState.Loading:
					break;
			}

			base.Draw(gameTime);
		}

		private void ForceDraw(GameTime gameTime)
		{
			BeginDraw();
			Draw(gameTime);
			EndDraw();
		}

		private void DrawLogo(GameTime gameTime, SpriteBatch sprite)
		{
			sprite.Begin(SpriteSortMode.BackToFront, BlendState.Additive);

			for (int i = 0; i < LogoScreenParticles.Length; i++)
			{
				LogoScreenParticles[i].Draw(gameTime, spriteBatch);
			}

			if (!exploded)
			{
				for (int i = 0; i < 5; i++)
				{
					sprite.Draw(logo, logoPosition, null, logoColor, logoRotation, new Vector2(logo.Width / 2, logo.Height / 2), logoScale, SpriteEffects.None, 0);
				}
			}
			sprite.End();
		}

		private void DrawGamePlay(GameTime gameTime)
		{
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

			spriteBatch.DrawString(font, gameplayText, textPos, Color.LimeGreen);

			spriteBatch.End();
		}

		protected void SetAlpha(int alpha)
		{
			// Keep in the range 0 to 255
			if (alpha < 0) alpha = 0;
			if (alpha > 255) alpha = 255;

			// Update the color
			Color c = logoColor;
			c.A = (byte)alpha;
			logoColor = c;
		}
	}
}
