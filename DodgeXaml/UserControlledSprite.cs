using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DodgeXaml
{
    public class UserControlledSprite : Sprite
    {
        // COMMENTED-OUT MOUSE SUPPORT
        MouseState prevMouseState;

        static Random rand = new Random();

        const int cooldownFrames = 12;
        int cooldowmRemaining = 0;

        

        // Get direction of sprite based on player input and speed
        public override Vector2 direction
        {
            get
            {
                Vector2 inputDirection = Vector2.Zero;

                // If player pressed arrow keys, move the sprite
                if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
                    inputDirection.X -= 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
                    inputDirection.X += 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
                    inputDirection.Y -= 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
                    inputDirection.Y += 1;

                // If player pressed the gamepad thumbstick, move the sprite
                GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
                if (gamepadState.ThumbSticks.Left.X != 0)
                    inputDirection.X += gamepadState.ThumbSticks.Left.X;
                if (gamepadState.ThumbSticks.Left.Y != 0)
                    inputDirection.Y -= gamepadState.ThumbSticks.Left.Y;

                // 通过摇杆改变player的direction
                inputDirection += TouchInputManager.Current.currentOSCState.ThumbSticks.Left;

                inputDirection += TouchInputManager.Current.currentOSCState.ThumbSticks.Right;


                return inputDirection * speed;
            }
        }

        public UserControlledSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, int scoreValue, int health)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, null, 0, health)
        {
            
        }

        public UserControlledSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, int millisecondsPerFrame,string collisionCueName,
            int scoreValue,int health)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame, null, 0, health)
        {
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // Move the sprite based on direction
            position += direction;

            // COMMENTED-OUT MOUSE SUPPORT
            //
            // If player moved the mouse, move the sprite
            //MouseState currMouseState = Mouse.GetState();
            //if (currMouseState.X != prevMouseState.X ||
            //    currMouseState.Y != prevMouseState.Y)
            //{
            //    position = new Vector2(currMouseState.X - SpriteManager.Current.player.frameSize.X * SpriteManager.Current.player.scale / 2,
            //        currMouseState.Y - SpriteManager.Current.player.frameSize.Y * SpriteManager.Current.player.scale / 2);
            //}
            //prevMouseState = currMouseState;

            // If sprite is off the screen, move it back within the game window
            if (position.X < 0)
                position.X = 0;
            if (position.Y < 0)
                position.Y = 0;
            if (position.X > clientBounds.Width - frameSize.X)
                position.X = clientBounds.Width - frameSize.X;
            if (position.Y > clientBounds.Height - frameSize.Y)
                position.Y = clientBounds.Height - frameSize.Y;


            Vector2 aim = GetAimDirection(SpriteManager.Current.player, 800f);
            if (aim.LengthSquared() > 0 && cooldowmRemaining <= 0)
            {
                cooldowmRemaining = cooldownFrames;
                float aimAngle = aim.ToAngle();
                Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

                float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f);
                Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, 11f);

                Vector2 offset = Vector2.Transform(new Vector2(35, -8), aimQuat);
                EntityManager.Add(new Bullet(CenterPosition + offset, vel));
                offset = Vector2.Transform(new Vector2(35, -16), aimQuat);
                EntityManager.Add(new Bullet(CenterPosition + offset, vel));

                offset = Vector2.Transform(new Vector2(35, 8), aimQuat);
                EntityManager.Add(new Bullet(CenterPosition + offset, vel));
                offset = Vector2.Transform(new Vector2(35, 16), aimQuat);
                EntityManager.Add(new Bullet(CenterPosition + offset, vel));
                Sound.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);

            }

            if (cooldowmRemaining > 0)
                cooldowmRemaining--;

            base.Update(gameTime, clientBounds);
        }
        /// <summary>
        /// 计算射击方向
        /// </summary>
        /// <param name="radius">感应半径</param>
        /// <returns>射击方向</returns>
        public Vector2 GetAimDirection(object obj, float radius)
        {
            Dictionary<double,Sprite> spritesWithinScopeDict = new Dictionary<double,Sprite>();
            Dictionary<double, Enemy> enemiesWithinScopeDict = new Dictionary<double, Enemy>();
            
            foreach (var sprite in SpriteManager.Current.spriteList)
            {
                if (!(sprite is UserControlledSprite))
                {
                    double distanceSquared = 0;
                    if (obj is Sprite)
                    {
                        distanceSquared = Vector2.DistanceSquared(((Sprite)obj).CenterPosition, sprite.GetPosition);
                    }
                    if (obj is Entity)
                    {
                        distanceSquared = Vector2.DistanceSquared(((Entity)obj).CenterPosition, sprite.GetPosition);
                        
                    }
                    if (distanceSquared < radius * radius)
                    {
                        if (!spritesWithinScopeDict.ContainsKey(distanceSquared))
                        {
                            spritesWithinScopeDict.Add(distanceSquared, sprite);
                        }
                    }
                }
            }
            Vector2 direction = Vector2.Zero;
            foreach (var enemy in EntityManager.enemies.Distinct())
            {
                double distanceSquared = 0;
                if (obj is Sprite)
                {
                    distanceSquared = Vector2.DistanceSquared(((Sprite)obj).CenterPosition, enemy.CenterPosition);
                }
                if (obj is Entity)
                {
                    distanceSquared = Vector2.DistanceSquared(((Entity)obj).CenterPosition, enemy.CenterPosition);
                }
                if (distanceSquared < radius * radius)
                {
                    if (!spritesWithinScopeDict.ContainsKey(distanceSquared))
                    {
                        enemiesWithinScopeDict.Add(distanceSquared, enemy);
                    }
                }
                
            }

            double minSprites;
            double minEnemies;

            if (spritesWithinScopeDict.Count <= 0)
            {
                minSprites = 0;
            }
            else
            {
                minSprites = spritesWithinScopeDict.Keys.Distinct().Min();
            }

            if (enemiesWithinScopeDict.Count <= 0)
            {
                minEnemies = 0;
            }
            else
            {
                minEnemies = enemiesWithinScopeDict.Keys.Distinct().Min();
            }

           
            if (spritesWithinScopeDict.Count <= 0 && enemiesWithinScopeDict.Count <= 0)
            {
                return Vector2.Zero;
            }


            if (minEnemies < minSprites)
            {
                if (enemiesWithinScopeDict.ContainsKey(minEnemies))
                {

                    Enemy enemyToShoot = enemiesWithinScopeDict[minEnemies];
                    if (obj is Sprite)
                    {
                        direction = enemyToShoot.CenterPosition - ((Sprite) obj).CenterPosition;
                    }
                    if (obj is Entity)
                    {
                        direction = enemyToShoot.CenterPosition - ((Entity) obj).CenterPosition;

                    }
                }
            }
            else
            {
                if (spritesWithinScopeDict.ContainsKey(minSprites))
                {
                    Sprite spriteToShoot = spritesWithinScopeDict[minSprites];
                    if (obj is Sprite)
                    {
                        direction = spriteToShoot.CenterPosition - ((Sprite) obj).CenterPosition;
                    }
                    if (obj is Entity)
                    {
                        direction = spriteToShoot.CenterPosition - ((Entity) obj).CenterPosition;

                    }
                }
            }

            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
        }

    }
}
