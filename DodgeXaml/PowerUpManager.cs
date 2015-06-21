using DodgeXaml.PowerUps;
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
using DodgeXaml.CommonHelper;

namespace DodgeXaml
{
    public class PowerUpManager : DrawableGameComponent
    {
        private Vector2 position;

        bool firstTimeGetRainEffect = true;
        bool firstTimeGetSnowEffect = true;
        bool firstTimeGetLightingEffect = true;


       

        enum Mode { BranchLightning}
        Mode mode;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
       
        KeyboardState keyState, lastKeyState;
        MouseState mouseState, lastMouseState;

        TouchCollection touches, previousTouches;
        List<ILightning> bolts = new List<ILightning>();

        private List<DrawableGameComponent> powerUpComponents = new List<DrawableGameComponent>();
        private Lighting lighting;
        private RainDrop rainDrop;
        //private SnowFall snowFall;
        private SnowFlakeFlyComponent snowFall;

        public PowerUpManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            lighting = new Lighting(Game1.Current);
            rainDrop = new RainDrop(Game1.Current);
            snowFall = new SnowFlakeFlyComponent(Game1.Current);
        }

        public enum PowerUp { Lighting, Raining, Snowing, Shaking, Others }
        public void UsePowerUp(PowerUp powerUp)
        {
            switch (powerUp)
            {
                case PowerUp.Lighting:
                    break;
                case PowerUp.Raining:
                    break;
                case PowerUp.Snowing:
                    break;
                case PowerUp.Shaking:
                    break;
                case PowerUp.Others:
                    break;
                default:
                    break;
            }
        }
       
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
           
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            UpdatePowerUps();

            base.Update(gameTime);
        }

        private void UpdatePowerUps()
        {
            if (Game1.currentScore < 20)
            {
                lighting.Enabled = false;
                lighting.Visible = false;
            }
            if (firstTimeGetLightingEffect && Game1.currentScore >= 200 && Game1.previousScore < 200)
            {
                firstTimeGetLightingEffect = false;

                XamlHelper.MakeToast("新技能", "获得了闪电特技！", "ms-appx:///Assets/Lightng.jpg");
                if (!Game1.Current.Components.Contains(lighting))
                {
                    Game1.Current.Components.Add(lighting);
                    lighting.Enabled = true;
                    lighting.Visible = true;
                    if (powerUpComponents.Contains(lighting))
                    {
                        powerUpComponents.Add(lighting);
                    }
                    
                }
                else
                {
                    lighting.Enabled = true;
                    lighting.Visible = true;
                    if (powerUpComponents.Contains(lighting))
                    {
                        powerUpComponents.Add(lighting);
                    }
                }
                
            }

            if (firstTimeGetRainEffect && Game1.currentScore >= 500 && Game1.previousScore < 500)
            {
                firstTimeGetRainEffect = false;

                XamlHelper.MakeToast("新技能", "获得了雨景特技！", "ms-appx:///Assets/rain.jpg");

                if (!Game1.Current.Components.Contains(rainDrop))
                {
                    Game1.Current.Components.Add(rainDrop);
                    rainDrop.Enabled = true;
                    rainDrop.Visible = true;
                    powerUpComponents.Add(rainDrop);
                }
                
            }

            if (firstTimeGetSnowEffect && Game1.currentScore >= 1000 && Game1.previousScore < 1000)
            {
                firstTimeGetSnowEffect = false;

                XamlHelper.MakeToast("新技能", "获得了暴雪特技！", "ms-appx:///Assets/rain.jpg");

                if (!Game1.Current.Components.Contains(snowFall))
                {
                    Game1.Current.Components.Add(snowFall);
                    snowFall.Enabled = true;
                    snowFall.Visible = true;
                    powerUpComponents.Add(snowFall);
                }
            }

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
