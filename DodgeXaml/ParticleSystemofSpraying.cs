using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace DodgeXaml
{
    //喷洒粒子类
    public class ParticleofSpraying
    {
        //粒子的初始位置、初速度和出生时间
        public Vector3 InitialPosition;
        public Vector3 InitialVelocity;
        public float BirthTime;


        //粒子的当前位置和当前速度
        public Vector3 Position;
        public Vector3 Velocity;


        public ParticleofSpraying(Vector3 pos, Vector3 vel, float bir)
        {
            //设置粒子的初始位置、初速度和出生时间
            InitialPosition = pos;
            InitialVelocity = vel;
            BirthTime = bir;

            //设置粒子的当前位置和当前速度
            Position = pos;
            Velocity = vel;
        }
    }


    //喷洒粒子系统类
    public class ParticleSystemofSpraying : DrawableGameComponent
    {
        //图形设备、效果和素材管理器件
        private GraphicsDevice graphics;
        private BasicEffect basicEffect;
        private SpriteBatch spriteBatch;
        private ContentManager Content;

        //喷洒粒子顶点数据
        private VertexBuffer verbufofspray;
        private VertexPositionColor[] arrpointofspray;
        private VertexDeclaration delofPosandColofSpray;

        private Matrix viewMatrix;
        private Matrix projectionMatrix;
        private Matrix worldMatrix;


        //喷洒粒子列表
        private List<ParticleofSpraying> Spray = new List<ParticleofSpraying>();
        private static int PARTICLENUM = 10000;

        //喷洒粒子使用的纹理
        private Texture2D Texture;

        //总时间
        private float TotalTime;


        private Random random = new Random();


        public ParticleSystemofSpraying(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public override void Initialize()
        {
            basicEffect = new BasicEffect(graphics);
            //获取图形设备、渲染喷洒粒子系统的效果和素材管理器

            //-------------------------------------------------------------------------
            //  Desc: 加载喷洒顶点数据
            //-------------------------------------------------------------------------
            //创建顶点声明
            delofPosandColofSpray = new VertexDeclaration(VertexPositionTexture.VertexDeclaration.GetVertexElements());
            //创建并填充喷洒粒子顶点缓冲区
            verbufofspray = new VertexBuffer(graphics, typeof (VertexPositionColor), 1, BufferUsage.None);
            arrpointofspray = new VertexPositionColor[1];
            arrpointofspray[0].Position = new Vector3(0, 0, 0);
            arrpointofspray[0].Color = Color.White;

            verbufofspray.SetData<VertexPositionColor>(arrpointofspray);

            //总时间置零
            TotalTime = 0.0f;

            //计算观察变换矩阵
            Vector3 camPosition = new Vector3(0.0f, 5.0f, 15.0f);
            Vector3 camTarget = new Vector3(0, 0, 0);
            Vector3 camUpVector = new Vector3(0, 1, 0);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, camUpVector);


            //计算投影变换矩阵
            float viewAngle = MathHelper.PiOver4;
            float aspectRatio = graphics.Viewport.AspectRatio;
            float nearPlane = 1.0f;
            float farPlane = 1000.0f;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(viewAngle, aspectRatio, nearPlane, farPlane);

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            //打开顶点颜色
            //basicEffect.VertexColorEnabled = true;

            //开启纹理，设置纹理
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = Texture;

            //设置相关渲染状态
            SetParticleRenderStates();

            //渲染喷洒粒子
            for (int i = 0; i < Spray.Count; i++)
            {
                Matrix rot, tran;
                tran = Matrix.CreateTranslation(2, 0.5f, 0);
                rot = Matrix.CreateRotationY(0.7f*MathHelper.Pi);

                Matrix worldMatrix = Matrix.CreateTranslation(Spray[i].Position)*rot*tran;
                basicEffect.World = worldMatrix;

                //设置观察变换矩阵，投影变换矩阵
                basicEffect.View = viewMatrix;
                basicEffect.Projection = projectionMatrix;

                //渲染喷洒粒子

                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    //设置顶点声明，设置顶点数据流，绘制图形
                    graphics.SetVertexBuffer(verbufofspray);

                    graphics.DrawPrimitives(PrimitiveType.LineList, 0, 1);
                }
                //basicEffect.End();
            }

            //恢复相关渲染状态
            ResetRenderStates();

            //提交
            graphics.Present();

            base.Draw(gameTime);
        }

        //更新喷洒粒子系统
        public override void Update(GameTime gameTime)
        {
            TotalTime += (float) gameTime.ElapsedGameTime.TotalSeconds;

            //遍历Spray列表中的所有对象，依次更新，如果喷洒粒子已落地，即位置的Y坐标值小于0时，移除该粒子
            int i = 0;
            while (i < Spray.Count)
            {
                if (Spray[i].Position.Y < 0.0f)
                    Spray.RemoveAt(i);

                //根据总时间和当前粒子的出生时间，计算该粒子的年龄；然后，进一步计算该粒子的当前位置和当前速度
                float age = TotalTime - Spray[i].BirthTime;

                float fGravity = -9.8f;
                Spray[i].Position = Spray[i].InitialPosition + Spray[i].InitialVelocity*age;
                Spray[i].Position.Y += (0.5f*fGravity)*(age*age);
                Spray[i].Velocity.Y = Spray[i].InitialVelocity.Y + fGravity*age;

                i++;
            }

            //添加喷洒粒子
            AddSprayingParticle();

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //加载喷洒粒子纹理
            Texture = Game.Content.Load<Texture2D>("particle");

            base.LoadContent();
        }


        //添加喷洒粒子。每一帧最多可添加15个喷洒粒子
        private void AddSprayingParticle()
        {
            for (int i = 0; i < 25; i++)
            {
                //如果当前喷洒粒子已满，则不需添加
                if (Spray.Count > PARTICLENUM)
                    return;

                float rand = (float) random.NextDouble()*2.0f - 1.0f;

                Vector3 vel = new Vector3();
                vel.X = 8.0f*rand;
                vel.Y = 0.0f;
                vel.Z = -10.0f;

                Spray.Add(new ParticleofSpraying(new Vector3(0, 1, 0), vel, TotalTime));
            }
        }

        //设置相关渲染状态
        private void SetParticleRenderStates()
        {

            //设置Alpha混合模式
            graphics.BlendState = new BlendState
            {
                AlphaBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One,
            };

            //renderState.DepthBufferWriteEnable = false;
            //设置剔除模式
            graphics.RasterizerState = new RasterizerState() {CullMode = CullMode.None};

        }

        //恢复相关渲染状态
        private void ResetRenderStates()
        {
            graphics.BlendState = new BlendState
            {
                AlphaSourceBlend = Blend.SourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha
            };

        }

    } 
}
