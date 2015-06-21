#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace DPSF.ParticleSystems
{
    /// <summary>
    /// Create a new Particle System class that inherits from a Default DPSF Particle System
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class GasFallParticleSystem : DefaultSprite3DBillboardParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GasFallParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 3000, 6000, "fire");
            LoadEvents();
            Emitter.ParticlesPerSecond = 20;
            Name = "Gas Fall";
        }

        public void LoadEvents()
        {
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingExternalForce);
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 500);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndQuickFadeOut, 100);

            InitialProperties.LifetimeMin = 20.0f;
            InitialProperties.LifetimeMax = 20.0f;
            InitialProperties.PositionMin = new Vector3(0, 200, 0);
            InitialProperties.PositionMax = new Vector3(0, 1000, 0);
            InitialProperties.StartSizeMin = 40000.0f;
            InitialProperties.StartSizeMax = 51000.0f;
            InitialProperties.EndSizeMin = 62000.0f;
            InitialProperties.EndSizeMax = 73000.0f;
            //InitialProperties.StartColorMin = Color.WhiteSmoke;
           // InitialProperties.StartColorMax = Color.WhiteSmoke;
            //InitialProperties.EndColorMin = Color.Yellow;
            //InitialProperties.EndColorMax = Color.Yellow;
            InitialProperties.InterpolateBetweenMinAndMaxColors = true;
            InitialProperties.RotationMin = 0;
            InitialProperties.RotationMax = 0.001f;
            InitialProperties.VelocityMin = new Vector3(-40000, 0, -40000);
            InitialProperties.VelocityMax = new Vector3(50000, 0, 50000);
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
            InitialProperties.RotationalVelocityMin = 0;
            InitialProperties.RotationalVelocityMax = 0.002f;
            InitialProperties.ExternalForceMin = new Vector3(0, 0, 0);
            InitialProperties.ExternalForceMax = new Vector3(0, 0, 0);
        }

        public void LoadExtraEvents()
        {
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingExternalForce);
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 500);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndQuickFadeOut, 100);

            ParticleEvents.AddNormalizedTimedEvent(0.6f, IncreaseHorizontalMovement);

            InitialProperties.LifetimeMin = 4.0f;
            InitialProperties.LifetimeMax = 5.0f;
            InitialProperties.PositionMin = new Vector3(0, 100, 0);
            InitialProperties.PositionMax = new Vector3(0, 100, 0);
            InitialProperties.StartSizeMin = 10.0f;
            InitialProperties.StartSizeMax = 10.0f;
            InitialProperties.EndSizeMin = 50.0f;
            InitialProperties.EndSizeMax = 50.0f;
            InitialProperties.StartColorMin = Color.WhiteSmoke;
            InitialProperties.StartColorMax = Color.WhiteSmoke;
            InitialProperties.EndColorMin = Color.WhiteSmoke;
            InitialProperties.EndColorMax = Color.WhiteSmoke;
            InitialProperties.RotationMin = 0;
            InitialProperties.RotationMax = MathHelper.TwoPi;
            InitialProperties.VelocityMin = new Vector3(-20, 0, 15);
            InitialProperties.VelocityMax = new Vector3(20, 0, 10);
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
            InitialProperties.RotationalVelocityMin = -MathHelper.TwoPi;
            InitialProperties.RotationalVelocityMax = MathHelper.TwoPi;
            InitialProperties.ExternalForceMin = new Vector3(0, -10, 0);
            InitialProperties.ExternalForceMax = new Vector3(0, -10, 0);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================
        protected void IncreaseHorizontalMovement(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
        {
            if (cParticle.Position.X > -50 && cParticle.Position.X < 50)
            {
                cParticle.Velocity.X *= Math.Abs((50.0f / cParticle.Position.X));
            }
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //===========================================================
        // Other Particle System Functions
        //===========================================================
    }
}