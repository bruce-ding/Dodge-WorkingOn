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
    public class RainDrop : DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        ParticleComponent particleComponent;
        private Random rnd;

        public RainDrop(Game game)
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

            Emitter rainDropEmitter = new Emitter();
            rainDropEmitter.Active = true;
            rainDropEmitter.TextureList.Add(Game1.Current.Content.Load<Texture2D>("Sprites\\raindrop"));
            rainDropEmitter.RandomEmissionInterval = new RandomMinMax(16.0d);
            rainDropEmitter.ParticleLifeTime = 1000;
            rainDropEmitter.ParticleDirection = new RandomMinMax(170);
            rainDropEmitter.ParticleSpeed = new RandomMinMax(10.0f);
            rainDropEmitter.ParticleRotation = new RandomMinMax(0);
            rainDropEmitter.RotationSpeed = new RandomMinMax(0f);
            rainDropEmitter.ParticleFader = new ParticleFader(false, true, 800);
            rainDropEmitter.ParticleScaler = new ParticleScaler(false, 1.0f);
            rainDropEmitter.Opacity = 255;

            particleComponent.particleEmitterList.Add(rainDropEmitter);
            
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
