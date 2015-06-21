using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Particles2DPipelineParticle;

namespace DodgeXaml.VisualEffects
{
    public class ParticleEffectManager : DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        // we want a sprite to represent our smokingEmitter
        Texture2D emitterSprite;

        // Here's the really fun part of the sample, the particle systems! These are
        // drawable game components, so we can just add them to the components
        // collection. Read more about each particle system in their respective source
        // files.
        ParticleSystem explosion;
        ParticleSystem smoke;
        ParticleSystem smokePlume;

        // For our Emitter test, we need both a ParticleEmitter and ParticleSystem
        ParticleEmitter emitter;
        ParticleSystem emitterSystem;

        // State is an enum that represents which effect we're currently demoing.
        enum State
        {
            Explosions,
            SmokePlume,
            Emitter
        };
        // the number of values in the "State" enum.
        const int NumStates = 3;
        State currentState = State.Explosions;

        // a timer that will tell us when it's time to trigger another explosion.
        const float TimeBetweenExplosions = 2.0f;
        float timeTillExplosion = 0.0f;

        // keep a timer that will tell us when it's time to add more particles to the
        // smoke plume.
        const float TimeBetweenSmokePlumePuffs = .5f;
        float timeTillPuff = 0.0f;

        // keep track of the last frame's keyboard and gamepad state, so that we know
        // if the user has pressed a button.
        KeyboardState lastKeyboardState;
        GamePadState lastGamepadState;
        
        public ParticleEffectManager(Game game)
            : base(game)
        {
            
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            // create the particle systems and add them to the components list.
            explosion = new ParticleSystem(game, "ExplosionSettings") { DrawOrder = ParticleSystem.AdditiveDrawOrder };
            game.Components.Add(explosion);

            smoke = new ParticleSystem(game, "ExplosionSmokeSettings") { DrawOrder = ParticleSystem.AlphaBlendDrawOrder };
            game.Components.Add(smoke);

            smokePlume = new ParticleSystem(game, "SmokePlumeSettings") { DrawOrder = ParticleSystem.AlphaBlendDrawOrder };
            game.Components.Add(smokePlume);

            emitterSystem = new ParticleSystem(game, "EmitterSettings") { DrawOrder = ParticleSystem.AlphaBlendDrawOrder };
            game.Components.Add(emitterSystem);
            emitter = new ParticleEmitter(emitterSystem, 60, new Vector2(400, 240));

            // enable the tap gesture for changing particle effects
            TouchPanel.EnabledGestures = GestureType.Tap;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            emitterSprite = Game.Content.Load<Texture2D>(Path.Combine("Particles2DPipelineParticle", "BlockEmitter"));

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // check the input devices to see if someone has decided they want to see
            // the other effect, if they want to quit.
            //HandleInput();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (currentState)
            {
                // if we should be demoing the explosions effect, check to see if it's
                // time for a new explosion.
                case State.Explosions:
                    UpdateExplosions(dt);
                    break;
                // if we're showing off the smoke plume, check to see if it's time for a
                // new puff of smoke.
                case State.SmokePlume:
                    UpdateSmokePlume(dt);
                    break;
                // if we're demoing the emitter attached to the mouse, update the emitter
                case State.Emitter:
                    UpdateEmitter(gameTime);
                    break;
            }

            // the base update will handle updating the particle systems themselves,
            // because we added them to the components collection.
            base.Update(gameTime);

            base.Update(gameTime);
        }

        // this function is called when we want to demo the use of the ParticleEmitter.
        // it figures out our new emitter location and updates the emitter which in 
        // turn handles creating any particles for the system.
        private void UpdateEmitter(GameTime gameTime)
        {
            // start with our current position
            Vector2 newPosition = emitter.Position;

            // Windows and Windows Phone use our Mouse class to update
            // the position of the emitter.
            MouseState mouseState = Mouse.GetState();
            newPosition = new Vector2(mouseState.X, mouseState.Y);


            // updating the emitter not only assigns a new location, but handles creating
            // the particles for our system based on the particlesPerSecond parameter of
            // the ParticleEmitter constructor.
            emitter.Update(gameTime, newPosition);
        }

        // this function is called when we want to demo the smoke plume effect. it
        // updates the timeTillPuff timer, and adds more particles to the plume when
        // necessary.
        private void UpdateSmokePlume(float dt)
        {
            timeTillPuff -= dt;
            if (timeTillPuff < 0)
            {
                Vector2 where = Vector2.Zero;
                // add more particles at the bottom of the screen, halfway across.
                where.X = Game.GraphicsDevice.Viewport.Width / 2;
                where.Y = Game.GraphicsDevice.Viewport.Height;
                smokePlume.AddParticles(where, Vector2.Zero);

                // and then reset the timer.
                timeTillPuff = TimeBetweenSmokePlumePuffs;
            }
        }

        // this function is called when we want to demo the explosion effect. it
        // updates the timeTillExplosion timer, and starts another explosion effect
        // when the timer reaches zero.
        private void UpdateExplosions(float dt)
        {
            if (SpriteManager.Current.collisionOccured)
            {

                Vector2 where = SpriteManager.Current.collisionPoint;

                // the overall explosion effect is actually comprised of two particle
                // systems: the fiery bit, and the smoke behind it. add particles to
                // both of those systems.
                explosion.AddParticles(where, Vector2.Zero);
                smoke.AddParticles(where, Vector2.Zero);
                SpriteManager.Current.collisionOccured = false;
            }

        }

        public override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin();

            // draw a sprite to represent our emitter for that state
            if (currentState == State.Emitter)
            {
                spriteBatch.Draw(
                    emitterSprite,
                    emitter.Position -
                        new Vector2(emitterSprite.Width / 2, emitterSprite.Height / 2),
                    Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        // This function will check to see if the user has just pushed the A button or
        // the space bar. If so, we should go to the next effect.
        private void HandleInput()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // check to see if someone has just released the space bar.            
            bool keyboardSpace =
                currentKeyboardState.IsKeyUp(Keys.Space) &&
                lastKeyboardState.IsKeyDown(Keys.Space);


            // check the gamepad to see if someone has just released the A button.
            bool gamepadA =
                currentGamePadState.Buttons.A == ButtonState.Pressed &&
                lastGamepadState.Buttons.A == ButtonState.Released;

            // check our gestures to see if someone has tapped the screen. we want
            // to read all available gestures even if a tap occurred so we clear
            // the queue.
            bool tapGesture = false;
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample sample = TouchPanel.ReadGesture();
                if (sample.GestureType == GestureType.Tap)
                {
                    tapGesture = true;
                }
            }


            // if either the A button or the space bar was just released, or the screen
            // was tapped, move to the next state. Doing modulus by the number of 
            // states lets us wrap back around to the first state.
            if (keyboardSpace || gamepadA || tapGesture)
            {
                currentState = (State)((int)(currentState + 1) % NumStates);
            }

            lastKeyboardState = currentKeyboardState;
            lastGamepadState = currentGamePadState;
        }
    }
}
