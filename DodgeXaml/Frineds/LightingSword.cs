using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLightningDemoLib;

namespace DodgeXaml.Frineds
{
    public class LightingSword
    {
        private Random rand = new Random();
        // 感应半径
        float radius = 800;

        List<LightningBolt> bolts = new List<LightningBolt>();

        public Rectangle CollisionRect;

        public Vector2 CenterPosition
        {
            get
            {
                return new Vector2(position.X + (float) textureImage.Width/2, position.Y + (float) textureImage.Height/2);
            }
        }

        public Vector2 ScreenSize
        {
            get { return Game1.Current.ScreenSize; }
        }

        private Texture2D textureImage;
        private float scale = 1;

        private Vector2 position;
        private Vector2 speed;


        public LightingSword(Texture2D textureImage, Vector2 position, Vector2 speed)
        {
            this.textureImage = textureImage;
            this.position = position;
            this.speed = speed;
        }

        public void Update(GameTime gameTime)
        {

            if (GetTargetPosition(radius) != this.CenterPosition)
            {
                if (bolts.Count <= 3)
                {
                    bolts.Add(new LightningBolt(this.CenterPosition, GetTargetPosition(radius)));
                }
            }

            foreach (var bolt in bolts)
            {
                bolt.Update();
            }

            bolts = bolts.Where(x => !x.IsComplete).ToList();
            for (int i = 0; i < SpriteManager.Current.spriteList.Count; i++)
            {
                if (! (SpriteManager.Current.spriteList[i] is UserControlledSprite))
                {
                    if (SpriteManager.Current.spriteList[i].CollisionRect.Intersects(this.CollisionRect))
                    {
                        // 防御性编程
                        if (SpriteManager.Current.spriteList.Contains(SpriteManager.Current.spriteList[i]))
                        {
                            SpriteManager.Current.spriteList.RemoveAt(i);
                        }
                    }
                }
            }

            // handle collisions between bullets and enemies
            for (int i = 0; i < EntityManager.enemies.Count; i++)
            {
                Enemy enemy = EntityManager.enemies[i];
                if (this.CollisionRect.Intersects(enemy.CollisionRect))
                {
                    enemy.HitPoints -= enemy.DamagePoint;
                    if (enemy.HitPoints <= 0)
                    {
                        enemy.IsExpired = true;
                    }
                }
               
            }

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(textureImage,
                position, null,
                Color.White, 0, Vector2.Zero,
                scale, SpriteEffects.None, 0);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);

            foreach (var bolt in bolts)
                bolt.Draw(spriteBatch);

            spriteBatch.End();
        }

        public Vector2 GetTargetPosition(float radius)
        {
            Dictionary<double, Sprite> spritesWithinScopeDict = new Dictionary<double, Sprite>();
            Dictionary<double, Enemy> enemiesWithinScopeDict = new Dictionary<double, Enemy>();
            double distanceSquared = 0;

            foreach (var sprite in SpriteManager.Current.spriteList)
            {
                if (!(sprite is UserControlledSprite))
                {
                    distanceSquared = Vector2.DistanceSquared(this.CenterPosition, sprite.GetPosition);

                    if (distanceSquared < radius*radius)
                    {
                        if (!spritesWithinScopeDict.ContainsKey(distanceSquared))
                        {
                            spritesWithinScopeDict.Add(distanceSquared, sprite);
                        }
                    }
                }
            }
            foreach (var enemy in EntityManager.enemies.Distinct())
            {
                
                distanceSquared = Vector2.DistanceSquared(this.CenterPosition, enemy.CenterPosition);

                if (distanceSquared < radius*radius)
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
                return this.CenterPosition;
            }

            if (minSprites < minEnemies)
            {
                // 此处的写法有问题，需要改正
                CollisionRect = new Rectangle((int)this.CenterPosition.X, (int)this.CenterPosition.Y, 20, (int)Math.Sqrt(minSprites));
                if (enemiesWithinScopeDict.ContainsKey(minSprites))
                {
                    return spritesWithinScopeDict[minSprites].CenterPosition;
                }
                else
                {
                    return this.CenterPosition;
                }

            }
            else
            {
                CollisionRect = new Rectangle((int)this.CenterPosition.X, (int)this.CenterPosition.Y, 20, (int)Math.Sqrt(minEnemies));
                if (enemiesWithinScopeDict.ContainsKey(minEnemies))
                {
                    return enemiesWithinScopeDict[minEnemies].CenterPosition;
                }
                else
                {
                    return this.CenterPosition;
                }
            }

        }

    }
}
