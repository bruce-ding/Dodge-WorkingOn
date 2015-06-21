using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGameLightningDemoLib;
using SharpDX.DirectWrite;
using X2DPE;
using X2DPE.Helpers;

namespace DodgeXaml.PowerUps
{
    public class SnowFall : DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        ParticleComponent particleComponent;
        private Random rnd;

        public SnowFall(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            
            rnd = new Random();

            particleComponent = Game1.Current.particleComponent;
            if (particleComponent == null)
            {
                particleComponent = new ParticleComponent(Game1.Current);
                if (!Game1.Current.Components.Contains(particleComponent))
                {
                    Game1.Current.Components.Add(particleComponent);
                }
            }
            
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Emitter testEmitter2 = new Emitter();
            testEmitter2.Active = true;
            testEmitter2.TextureList.Add(Game1.Current.Content.Load<Texture2D>("Sprites\\snow1"));
            testEmitter2.TextureList.Add(Game1.Current.Content.Load<Texture2D>("Sprites\\snow2"));

            testEmitter2.RandomEmissionInterval = new RandomMinMax(16.0d);
            testEmitter2.ParticleLifeTime = 2000;
            testEmitter2.ParticleDirection = new RandomMinMax(180);
            testEmitter2.ParticleSpeed = new RandomMinMax(5.0f);
            testEmitter2.ParticleRotation = new RandomMinMax(0);
            testEmitter2.RotationSpeed = new RandomMinMax(0f);
            testEmitter2.ParticleFader = new ParticleFader(false, true, 800);
            testEmitter2.ParticleScaler = new ParticleScaler(false, 0.5f);
            testEmitter2.Opacity = 255;

            particleComponent.particleEmitterList.Add(testEmitter2);
            
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            Emitter t2 = particleComponent.particleEmitterList[0];
            t2.Position = new Vector2((float)rnd.NextDouble() * (Game1.Current.GraphicsDevice.Viewport.Width), 0);
            if (t2.EmittedNewParticle)
            {
                float f = MathHelper.ToRadians(t2.LastEmittedParticle.Direction + 180);
                t2.LastEmittedParticle.Rotation = f;
            }
          
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here

           base.Draw(gameTime);
        }

   }
}
