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

namespace DodgeXaml.PowerUps
{
    public class Lighting : DrawableGameComponent
    {

        private Vector2 position;

        private int level;

        public int Level 
        { 
            get
            {
                return level;
            }
            set
            {
                level = value;
            }
        }

        private Dictionary<int, int> levelRadiusDict = new Dictionary<int,int>();

        public Lighting(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            levelRadiusDict.Add(1,200);
            levelRadiusDict.Add(2, 300);
            levelRadiusDict.Add(3, 400);
            levelRadiusDict.Add(4, 500);
            levelRadiusDict.Add(5, 600);
            levelRadiusDict.Add(6, 700);

        }

        enum Mode { BranchLightning}
        Mode mode;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
       
        KeyboardState keyState, lastKeyState;
        MouseState mouseState, lastMouseState;
        

        TouchCollection touches, previousTouches;
        List<ILightning> bolts = new List<ILightning>();

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Art.Load(Game.Content);
            
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            //position = SpriteManager.Current.GetPlayerPosition();
            position = SpriteManager.Current.player.CenterPosition;

            lastKeyState = keyState;
            keyState = Keyboard.GetState();
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            mode = Mode.BranchLightning;

            var screenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            previousTouches = touches;
            touches = TouchPanel.GetState();
            for (int i = 0; i < touches.Count; i++)
            {
                if (touches[i].State != TouchLocationState.Pressed)
                {
                    continue;
                }
                if (touches.Count == 1)
                {
                    bolts.Add(new LightningBolt(screenSize / 2, touches[i].Position));
                }
                else
                {
                    if (i > 0)
                        bolts.Add(new LightningBolt(touches[i - 1].Position, touches[i].Position));
                }
            }

            switch (mode)
            {
               case Mode.BranchLightning:
                    if (WasPressed(Keys.Space) || WasTapped())
                    {
                        bolts.Add(new BranchLightning(position, Vector2.Add(position, new Vector2(-200, 0))));
                        bolts.Add(new BranchLightning(position, Vector2.Add(position, new Vector2(0, -200))));
                        bolts.Add(new BranchLightning(position, Vector2.Add(position, new Vector2(200, 0))));
                        bolts.Add(new BranchLightning(position, Vector2.Add(position, new Vector2(0, 200))));
                        Game1.currentScore -= 20;
                        MonoGameLightningDemoLib.Art.LightingSound.Play();
                        for(int i = 0; i < SpriteManager.Current.spriteList.Count; ++i)
                        {
                            if (Vector2.DistanceSquared(SpriteManager.Current.spriteList[i].GetPosition, SpriteManager.Current.player.GetPosition) <= 200 * 200)
                            {
                                SpriteManager.Current.spriteList.RemoveAt(i);
                                --i;
                            }
                        }
                        GamePage.WasLightingTapped = false;
                    }
                    break;
            }

            foreach (var bolt in bolts)
                bolt.Update();

            bolts = bolts.Where(x => !x.IsComplete).ToList();

            base.Update(gameTime);
        }

        // return true if a key was pressed down this frame
        bool WasPressed(Keys key)
        {
            return keyState.IsKeyDown(key) && lastKeyState.IsKeyUp(key);
        }

        // return true if the left mouse button was clicked down this frame
        bool WasClicked()
        {
            return mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released;
        }

        bool WasTapped()
        {
            return GamePage.WasLightingTapped == true;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            
            DrawLightning();

           base.Draw(gameTime);
        }

        void DrawLightning()
        {
            // we use SpriteSortMode.Texture to improve performance
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);

            foreach (var bolt in bolts)
                bolt.Draw(spriteBatch);

            spriteBatch.End();
           
        }

   }
}
