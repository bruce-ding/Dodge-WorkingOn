using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeXaml.Frineds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Supernova.Particles2D;
using Supernova.Particles2D.Modifiers.Movement.Gravity;

namespace DodgeXaml
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpriteManager : DrawableGameComponent
    {
        public static SpriteManager Current { get; private set; }

        // SpriteBatch for drawing
        private SpriteBatch spriteBatch;

        //private ParticleEffect2D particleEffect;
        private ParticleEffect2D multiGravityPoint;

        private readonly Random _randomizer = new Random();

        // A sprite for the player and a list of automated sprites
        public UserControlledSprite player;
        public List<Sprite> spriteList = new List<Sprite>();

        // Variables for spawning new enemies
        private int enemySpawnMinMilliseconds = 1000;
        private int enemySpawnMaxMilliseconds = 2000;
        private int enemyMinSpeed = 1;
        private int enemyMaxSpeed = 3;
        private int nextSpawnTime = 0;

        // Chance of spawning different enemies
        private int likelihoodAutomated = 75;
        private int likelihoodChasing = 20;
        private int likelihoodEvading = 5;

        // Anemy Collision SoundEffect Name
        private string collisionName = "explosivecollision";

        // Scoring
        private int automatedSpritePointValue = 10;
        private int chasingSpritePointValue = 20;
        private int evadingSpritePointValue = 0;

        // player的生命值
        //public List<AutomatedSprite> livesList = new List<AutomatedSprite>();
        // 因为生命值可能会太多，因此只需绘制一次
        private AutomatedSprite playerLivesSprite;

        //Spawn time variables
        private int nextSpawnTimeChange = 5000;
        private int timeSinceLastSpawnTimeChange = 0;

        // Powerup stuff
        private int powerUpExpiration = 0;

        // 爆炸效果
        private Explosion _explosion;
        private List<ParticleData> _particleList;

        private Vector2 emitPositionOfMultyPointTransform;

        private LightingSword lightingSword;

        public Vector2 collisionPoint;

        public bool collisionOccured;

        public SpriteManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            Current = this;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // Initialize spawn time
            ResetSpawnTime();

            emitPositionOfMultyPointTransform = new Vector2(GraphicsDevice.Viewport.Width/2f,
                GraphicsDevice.Viewport.Height/2f);

            _explosion = new Explosion();
            _particleList = new List<ParticleData>();
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            DodgeXaml.Art.Load(Game.Content);
            MonoGameLightningDemoLib.Art.Load(Game.Content);
            Sound.Load(Game.Content);

            player = new UserControlledSprite(
                Game.Content.Load<Texture2D>(@"Images/threerings"),
                new Vector2(Game.Window.ClientBounds.Width/2f,
                    Game.Window.ClientBounds.Height/2f),
                new Point(75, 75), 10, new Point(0, 0),
                new Point(6, 8), new Vector2(6, 6), 0, 0);


            lightingSword = new LightingSword(Game.Content.Load<Texture2D>(@"Images/Seeker"),
                new Vector2((float) Game.Window.ClientBounds.Width/2,
                (float) Game.Window.ClientBounds.Height/2),
                Vector2.Zero);

            playerLivesSprite = new AutomatedSprite(
                Game.Content.Load<Texture2D>(@"images\threerings"),
                new Vector2(10, 35), new Point(75, 75), 10,
                new Point(0, 0), new Point(6, 8), Vector2.Zero,
                null, 0, 0, .5f);

            //particleEffect = ParticleEffect2DFactory.Initialize(100000, 4000)
            //                                       .SetMaxParticleSpeed(1f)
            //                                       .SetEmitAmount(300)
            //                                       .AddTexture(Texture2DFactory.New(GraphicsDevice, 1, 1, Color.Red))
            //                                       .AddTexture(Texture2DFactory.New(GraphicsDevice, 2, 2, Color.Red))
            //                                       .AddModifier(new AlphaAgeTransform())
            //                                       .Create();

            var modifierGravityPoint1 = new GravityPoint();
            modifierGravityPoint1.Position = new Vector2(400, 120);
            modifierGravityPoint1.Radius = 500f;
            modifierGravityPoint1.Strength = 5f;

            var modifierGravityPoint2 = new GravityPoint();
            modifierGravityPoint2.Position = new Vector2(200, 360);
            modifierGravityPoint2.Radius = 500f;
            modifierGravityPoint2.Strength = 5f;

            var modifierGravityPoint3 = new GravityPoint();
            modifierGravityPoint3.Position = new Vector2(600, 360);
            modifierGravityPoint3.Radius = 500f;
            modifierGravityPoint3.Strength = 5f;

            var modifierColorCycleTransform = new Supernova.Particles2D.Modifiers.Color.ColorCycleTransform();
            modifierColorCycleTransform.CycleLength = 5000f;
            modifierColorCycleTransform.ColorList = new List<Color>() {Color.DodgerBlue, Color.LimeGreen};

            multiGravityPoint = ParticleEffect2DFactory.Initialize(1000, 800000)
                .SetMaxParticleSpeed(5f)
                .SetEmitAmount(1000)
                .AddTexture(Game.Content.Load<Texture2D>(@"Textures\bigDot"))
                .AddTexture(Game.Content.Load<Texture2D>(@"Textures\particle"))
                .AddTexture(Game.Content.Load<Texture2D>(@"Textures\glowParticle"))
                .AddModifier(modifierGravityPoint1)
                .AddModifier(modifierGravityPoint2)
                .AddModifier(modifierGravityPoint3)
                .AddModifier(modifierColorCycleTransform)
                .Create();

            _explosion.Texture = Game.Content.Load<Texture2D>(@"Images\explosion");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Time to spawn enemy?
            nextSpawnTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (nextSpawnTime < 0)
            {
                SpawnEnemy();

                // Reset spawn timer
                ResetSpawnTime();
            }

            UpdateSprites(gameTime);


            // Adjust sprite spawn times
            AdjustSpawnTimes(gameTime);

            // Expire Powerups?
            CheckPowerUpExpiration(gameTime);

            // 处理爆炸效果
            if (_particleList.Count > 0)
                UpdateParticles(gameTime);

            //particleEffect.Update((float)gameTime.TotalGameTime.TotalMilliseconds, (float)gameTime.ElapsedGameTime.TotalSeconds, false);
            
            //背景特效
            multiGravityPoint.Emit((float) gameTime.TotalGameTime.TotalMilliseconds, emitPositionOfMultyPointTransform);
            multiGravityPoint.Update((float) gameTime.TotalGameTime.TotalMilliseconds,
                (float) gameTime.ElapsedGameTime.TotalSeconds, false);

            // 处理Bullet
            EntityManager.Update(gameTime);

            EnemySpawner.Update();

            lightingSword.Update(gameTime);
            
            base.Update(gameTime);
        }

        private void UpdateSprites(GameTime gameTime)
        {
            // Update player
            player.Update(gameTime, Game.Window.ClientBounds);

            // Update all sprites
            if (spriteList.Count > 0)
            {
                for (int i = 0; i < spriteList.Count; ++i)
                {
                    Sprite s = spriteList[i];

                    s.Update(gameTime, Game.Window.ClientBounds);

                    // 分不同精灵类型，处理与Player的碰撞
                    if (s.CollisionRect.Intersects(player.CollisionRect))
                    {
                        // Play collision sound
                        if (s.collisionCueName != null)
                            ((Game1) Game).PlaySound(s.collisionCueName);

                        // If collided with AutomatedSprite
                        // remove a life from the player
                        if (s is AutomatedSprite)
                        {
                            //if (livesList.Count > 0)
                            //{
                            //    livesList.RemoveAt(livesList.Count - 1);
                            //    --((Game1)Game).NumberLivesRemaining;
                            //}
                            if (((Game1) Game).NumberLivesRemaining > 0)
                            {
                                --((Game1) Game).NumberLivesRemaining;
                            }
                        }
                        else if (s.collisionCueName == "pluscollision")
                        {
                            // Collided with plus - start plus power-up
                            powerUpExpiration = 5000;
                            player.ModifyScale(2);
                        }
                        else if (s.collisionCueName == "skullcollision")
                        {
                            // Collided with skull - start skull power-up
                            powerUpExpiration = 5000;
                            player.ModifySpeed(.5f);
                        }
                        else if (s.collisionCueName == "boltcollision")
                        {
                            // Collided with bolt - start bolt power-up
                            powerUpExpiration = 5000;
                            player.ModifySpeed(2);
                        }

                        // Remove collided sprite from the game
                        spriteList.RemoveAt(i);
                        --i;

                        //Vector2 emitPosition = new Vector2(
                        //        (s.collisionRect.X + player.collisionRect.X) / 2,
                        //        (s.collisionRect.Y + player.collisionRect.Y) / 2);
                        //particleEffect.Emit((float)gameTime.TotalGameTime.TotalMilliseconds, emitPosition);
                    }

                    for (int j = 0; j < spriteList.Count; j++)
                    {
                        Sprite otherSprite = spriteList[j];
                        //otherSprite.Update(gameTime, Game.Window.ClientBounds);

                        if (i != j && s.CollisionRect.Intersects(otherSprite.CollisionRect))
                        {
                            // Play collision sound
                            ((Game1) Game).PlaySound(collisionName);

                            ((Game1) Game).AddScore(s.ScoreValue, gameTime);
                            ((Game1) Game).AddScore(otherSprite.ScoreValue, gameTime);

                            spriteList.RemoveAt(i);
                            spriteList.Remove(otherSprite);
                            --i;
                            --j;

                            collisionOccured = true;
                            collisionPoint = new Vector2(
                                 (s.CollisionRect.X + otherSprite.CollisionRect.X) / 2f,
                                 (s.CollisionRect.Y + otherSprite.CollisionRect.Y) / 2f);
                            AddExplosion(collisionPoint, 10, 150.0f, 3000.0f, gameTime);
                            

                        }
                    }

                    // Remove object if it is out of bounds
                    if (s.IsOutOfBounds(Game.Window.ClientBounds))
                    {
                        ((Game1) Game).AddScore(s.ScoreValue, gameTime);
                        spriteList.Remove(s);

                        // Just for precaution预防性编程
                        if (i - 1 >= 0)
                        {
                            --i;
                        }

                    }

                    // 处理子弹与精灵的碰撞
                    if (!(s is UserControlledSprite))
                    {
                        for (int n = 0; n < EntityManager.bullets.Count; n++)
                        {
                            Bullet bullet = EntityManager.bullets[n];
                            if (s.CollisionRect.Intersects(bullet.CollisionRect))
                            {
                                // Just for precaution
                                if (i >= 0)
                                {
                                    ((Game1) Game).AddScore(s.ScoreValue, gameTime);
                                    if (spriteList.Contains(s))
                                    {
                                        spriteList.RemoveAt(i);
                                    }

                                }
                                EntityManager.bullets.RemoveAt(n);

                                if ((i - 1) >= 0)
                                {
                                    --i;
                                    --n;
                                }
                            }
                        }
                    }

                }
            }

            // Update lives-list sprites
            //foreach (Sprite sprite in livesList)
            //{
            //    sprite.Update(gameTime, Game.Window.ClientBounds);

            // 单独更新
            playerLivesSprite.Update(gameTime, Game.Window.ClientBounds);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // Draw the player
            player.Draw(gameTime, spriteBatch);

            // Draw all sprites
            foreach (Sprite s in spriteList)
            {
                s.Draw(gameTime, spriteBatch);
            }

            // Draw player lives
            //foreach (Sprite sprite in livesList)
            //{
            //    sprite.Draw(gameTime, spriteBatch);
            //}

            playerLivesSprite.Draw(gameTime, spriteBatch);

            spriteBatch.DrawString(Game1.Current.scoreFont,
                "Lives: " + Game1.Current.NumberLivesRemaining,
                new Vector2(50, 37), Color.DarkBlue,
                0, Vector2.Zero,
                1, SpriteEffects.None, 1);
            spriteBatch.End();

            spriteBatch.Begin();
            //particleEffect.Draw(spriteBatch);
            multiGravityPoint.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap,
                DepthStencilState.None, RasterizerState.CullNone);
            DrawExplosion();
            spriteBatch.End();

            // 处理Bullet
            spriteBatch.Begin();
            EntityManager.Draw(spriteBatch);
            spriteBatch.End();

            lightingSword.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }

        private void ResetSpawnTime()
        {
            // Set the next spawn time for an enemy
            nextSpawnTime = ((Game1) Game).rnd.Next(
                enemySpawnMinMilliseconds,
                enemySpawnMaxMilliseconds);
        }

        private void SpawnEnemy()
        {
            Vector2 speed = Vector2.Zero;
            Vector2 position = Vector2.Zero;

            // Default frame size
            Point frameSize = new Point(75, 75);

            // Randomly choose which side of the screen to place enemy,
            // then randomly create a position along that side of the screen
            // and randomly choose a speed for the enemy
            switch (((Game1) Game).rnd.Next(4))
            {
                case 0: // LEFT to RIGHT
                    position = new Vector2(
                        -frameSize.X, ((Game1) Game).rnd.Next(0,
                            Game.GraphicsDevice.PresentationParameters.BackBufferHeight
                            - frameSize.Y));
                    speed = new Vector2(((Game1) Game).rnd.Next(
                        enemyMinSpeed,
                        enemyMaxSpeed), 0);
                    break;
                case 1: // RIGHT to LEFT
                    position = new
                        Vector2(
                        Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                        ((Game1) Game).rnd.Next(0,
                            Game.GraphicsDevice.PresentationParameters.BackBufferHeight
                            - frameSize.Y));

                    speed = new Vector2(-((Game1) Game).rnd.Next(
                        enemyMinSpeed, enemyMaxSpeed), 0);
                    break;
                case 2: // BOTTOM to TOP
                    position = new Vector2(((Game1) Game).rnd.Next(0,
                        Game.GraphicsDevice.PresentationParameters.BackBufferWidth
                        - frameSize.X),
                        Game.GraphicsDevice.PresentationParameters.BackBufferHeight);

                    speed = new Vector2(0,
                        -((Game1) Game).rnd.Next(enemyMinSpeed,
                            enemyMaxSpeed));
                    break;
                case 3: // TOP to BOTTOM
                    position = new Vector2(((Game1) Game).rnd.Next(0,
                        Game.GraphicsDevice.PresentationParameters.BackBufferWidth
                        - frameSize.X), -frameSize.Y);

                    speed = new Vector2(0,
                        ((Game1) Game).rnd.Next(enemyMinSpeed,
                            enemyMaxSpeed));
                    break;
            }

            #region 根据各种精灵产生的可能性选择生成相应的精灵
            // Get random number between 0 and 99
            int random = ((Game1) Game).rnd.Next(100);
            if (random < likelihoodAutomated)// 75
            {
                // Create an AutomatedSprite.
                // Get new random number to determine whether to
                // create a three-blade or four-blade sprite.
                if (((Game1) Game).rnd.Next(2) == 0)
                {
                    // Create a four-blade enemy
                    spriteList.Add(
                        new AutomatedSprite(
                            Game.Content.Load<Texture2D>(@"images\fourblades"),
                            position, new Point(75, 75), 10, new Point(0, 0),
                            new Point(6, 8), speed, "fourbladescollision",
                            automatedSpritePointValue, 0));
                }
                else
                {
                    // Create a three-blade enemy
                    spriteList.Add(
                        new AutomatedSprite(
                            Game.Content.Load<Texture2D>(@"images\threeblades"),
                            position, new Point(75, 75), 10, new Point(0, 0),
                            new Point(6, 8), speed, "threebladescollision",
                            automatedSpritePointValue, 0));
                }
            }
            else if (random < likelihoodAutomated +
                     likelihoodChasing)
            {
                // Create a ChasingSprite.
                // Get new random number to determine whether
                // to create a skull or a plus sprite.
                if (((Game1) Game).rnd.Next(2) == 0)
                {
                    // Create a skull
                    spriteList.Add(
                        new ChasingSprite(
                            Game.Content.Load<Texture2D>(@"images\skullball"),
                            position, new Point(75, 75), 10, new Point(0, 0),
                            new Point(6, 8), speed, "skullcollision", this,
                            chasingSpritePointValue, 0));
                }
                else
                {
                    // Create a plus
                    spriteList.Add(
                        new ChasingSprite(
                            Game.Content.Load<Texture2D>(@"images\plus"),
                            position, new Point(75, 75), 10, new Point(0, 0),
                            new Point(6, 4), speed, "pluscollision", this,
                            chasingSpritePointValue, 0));
                }
            }
            else
            {
                // Create an EvadingSprite
                spriteList.Add(
                    new EvadingSprite(
                        Game.Content.Load<Texture2D>(@"images\bolt"),
                        position, new Point(75, 75), 10, new Point(0, 0),
                        new Point(6, 8), speed, "boltcollision", this,
                        .75f, 150, evadingSpritePointValue, 0));
            }
#endregion
        }

        // Return current position of the player sprite
        public Vector2 GetPlayerPosition()
        {
            return player.GetPosition;
        }

        private void AdjustSpawnTimes(GameTime gameTime)
        {
            // If the spawn max time is > 500 milliseconds
            // decrease the spawn time if it is time to do
            // so based on the spawn-timer variables
            if (enemySpawnMaxMilliseconds > 500)
            {
                timeSinceLastSpawnTimeChange += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastSpawnTimeChange > nextSpawnTimeChange)
                {
                    timeSinceLastSpawnTimeChange -= nextSpawnTimeChange;
                    if (enemySpawnMaxMilliseconds > 1000)
                    {
                        enemySpawnMaxMilliseconds -= 100;
                        enemySpawnMinMilliseconds -= 100;
                    }
                    else
                    {
                        enemySpawnMaxMilliseconds -= 10;
                        enemySpawnMinMilliseconds -= 10;
                    }
                }
            }
        }

        private void CheckPowerUpExpiration(GameTime gameTime)
        {
            // Is a power-up active?
            if (powerUpExpiration > 0)
            {
                // Decrement power-up timer
                powerUpExpiration -= gameTime.ElapsedGameTime.Milliseconds;
                if (powerUpExpiration <= 0)
                {
                    // If power-up timer has expired, end all power-ups
                    powerUpExpiration = 0;
                    player.ResetScale();
                    player.ResetSpeed();
                }
            }
        }

        private void DrawExplosion()
        {
            for (int i = 0; i < _particleList.Count; i++)
            {
                ParticleData particle = _particleList[i];
                spriteBatch.Draw(_explosion.Texture, particle.Position, null, particle.ModColor, i,
                    new Vector2(256, 256), particle.Scaling, SpriteEffects.None, 1);
            }
        }

        private void AddExplosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge,
            GameTime gameTime)
        {
            for (var i = 0; i < numberOfParticles; i++)
                AddExplosionParticle(explosionPos, size, maxAge, gameTime);
        }

        private void UpdateParticles(GameTime gameTime)
        {
            var now = (float) gameTime.TotalGameTime.TotalMilliseconds;
            for (var i = _particleList.Count - 1; i >= 0; i--)
            {
                var particle = _particleList[i];
                var timeAlive = now - particle.BirthTime;

                if (timeAlive > particle.MaxAge)
                {
                    _particleList.RemoveAt(i);
                }
                else
                {
                    var relAge = timeAlive / particle.MaxAge;
                    particle.Position = 0.5f*particle.Accelaration*relAge*relAge 
                        + particle.Direction*relAge + particle.OrginalPosition;

                    var invAge = 1.0f - relAge;
                    particle.ModColor = new Color(new Vector4(invAge, invAge, invAge, invAge));

                    var positionFromCenter = particle.Position - particle.OrginalPosition;
                    var distance = positionFromCenter.Length();
                    particle.Scaling = (50.0f + distance)/200.0f;

                    _particleList[i] = particle;
                }
            }
        }

        private void AddExplosionParticle(Vector2 explosionPos, float explosionSize, float maxAge, GameTime gameTime)
        {
            var particle = new ParticleData {OrginalPosition = explosionPos};

            particle.Position = particle.OrginalPosition;

            particle.BirthTime = (float) gameTime.TotalGameTime.TotalMilliseconds;
            particle.MaxAge = maxAge;
            particle.Scaling = 0.25f;
            particle.ModColor = Color.Red;

            var particleDistance = (float) _randomizer.NextDouble()*explosionSize;
            var displacement = new Vector2(particleDistance, 0);
            var angle = MathHelper.ToRadians(_randomizer.Next(360));
            displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

            particle.Direction = displacement*2.0f;
            particle.Accelaration = -particle.Direction;

            _particleList.Add(particle);
        }

    }
}