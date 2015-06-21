using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DodgeXaml.PowerUps
{
    public class SnowFlakeFlyComponent : DrawableGameComponent
    {
        //图形设备、效果和素材管理器
        private Effect EffectofSnow;

        private Matrix viewMatrix;

        private Matrix projectionMatrix;

        //粒子系统所使用的顶点缓冲区、顶点数组、顶点声明及所使用的纹理
        private VertexBuffer verbufofsnow;                 //雪花顶点缓冲区
        private VertexPositionTexture[] arrpointofsnow;    //雪花顶点数组
        private VertexDeclaration delofPosandTexofSnow;    //雪花顶点声明

        private Texture2D snow1;                           //雪花纹理
        private Texture2D snow2;
        private Texture2D snow3;

        //雪花粒子数组
        ParticleofSnowing[] ParticlesofSnow;                  //雪花粒子数组

        private static int PARTICLENUM = 1000;

        //总时间
        float TotalTime;

        public SnowFlakeFlyComponent(Game game)
            : base(game)
        {
            // TODO: 在此处构造任何子组件
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;
        }

        /// <summary>
        /// 允许游戏组件在开始运行之前执行其所需的任何初始化。
        /// 游戏组件能够在此时查询任何所需服务和加载内容。
        /// </summary>
        public override void Initialize()
        {
            // TODO: 在此处添加初始化代码

            projectionMatrix = CalculateProjectionMatrix();

            //-------------------------------------------------------------------------
            //  Desc: 加载雪花顶点数据
            //-------------------------------------------------------------------------
            //创建顶点声明
            delofPosandTexofSnow = new VertexDeclaration(VertexPositionTexture.VertexDeclaration.GetVertexElements());
            //创建并填充顶点缓冲区
            verbufofsnow = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.None);
            arrpointofsnow = new VertexPositionTexture[4];
            arrpointofsnow[0].Position = new Vector3(-1, 0, 0);
            arrpointofsnow[0].TextureCoordinate = new Vector2(0, 1);
            arrpointofsnow[1].Position = new Vector3(-1, 2, 0);
            arrpointofsnow[1].TextureCoordinate = new Vector2(0, 0);
            arrpointofsnow[2].Position = new Vector3(1, 0, 0);
            arrpointofsnow[2].TextureCoordinate = new Vector2(1, 1);
            arrpointofsnow[3].Position = new Vector3(1, 2, 0);
            arrpointofsnow[3].TextureCoordinate = new Vector2(1, 0);
            verbufofsnow.SetData<VertexPositionTexture>(arrpointofsnow);

            //填充雪花粒子数组
            ParticlesofSnow = new ParticleofSnowing[PARTICLENUM];
            Random r = new Random();
            for (int i = 0; i < PARTICLENUM; i++)
            {
                float x = r.Next() % 200 - 100;
                float y = r.Next() % 250;
                float z = r.Next() % 200 - 100;
                Vector3 pos = new Vector3(x, y, z);

                float pitch = (r.Next() % 100) / 50.0f * (float)Math.PI;
                float yaw = (r.Next() % 100) / 50.0f * (float)Math.PI;
                float roll = (r.Next() % 100) / 50.0f * (float)Math.PI;
                Vector3 rot = new Vector3(pitch, yaw, roll);

                float dspeed = 10.0f + r.Next() % 10;
                float rspeed = 1.0f + r.Next() % 10 / 10.0f;
                int texindex = r.Next() % 3;

                ParticlesofSnow[i] = new ParticleofSnowing(pos, rot, dspeed, rspeed, texindex);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //加载渲染雪花粒子的效果
            EffectofSnow = Game.Content.Load<Effect>("SnowEffect/Snow");
            //加载雪花粒子使用的纹理
            snow1 = Game.Content.Load<Texture2D>("SnowEffect/snow1");
            snow2 = Game.Content.Load<Texture2D>("SnowEffect/snow2");
            snow3 = Game.Content.Load<Texture2D>("SnowEffect/snow3");

            base.LoadContent();
        }

        /// <summary>
        /// 允许游戏组件进行自我更新。
        /// </summary>
        /// <param name="gameTime">提供计时值的快照。</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: 在此处添加更新代码
            float fElapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < PARTICLENUM; i++)
            {
                ParticlesofSnow[i].Position.Y -= ParticlesofSnow[i].DropSpeed * fElapsedTime;
                if (ParticlesofSnow[i].Position.Y < 0)
                    ParticlesofSnow[i].Position.Y = 250.0f;

                ParticlesofSnow[i].AngelofRotation.X += ParticlesofSnow[i].RotationSpeed * fElapsedTime;
                ParticlesofSnow[i].AngelofRotation.Y += ParticlesofSnow[i].RotationSpeed * fElapsedTime;
                ParticlesofSnow[i].AngelofRotation.Z += ParticlesofSnow[i].RotationSpeed * fElapsedTime;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            viewMatrix = CalculateViewMatrix(gameTime);

            //遍历每个雪花粒子
            for (int i = 0; i < PARTICLENUM; i++)
            {

                //计算当前雪花粒子的世界变换矩阵
                Matrix worldMatrix, matTran, matPitch, matYaw, matRoll;
                matTran = Matrix.CreateTranslation(ParticlesofSnow[i].Position);
                matPitch = Matrix.CreateRotationX(ParticlesofSnow[i].AngelofRotation.X);
                matYaw = Matrix.CreateRotationY(ParticlesofSnow[i].AngelofRotation.Y);
                matRoll = Matrix.CreateRotationZ(ParticlesofSnow[i].AngelofRotation.Z);
                worldMatrix = matYaw * matPitch * matRoll * matTran;

                //计算当前雪花粒子的组合变换矩阵
                Matrix WorldViewProjMatrix = worldMatrix * viewMatrix * projectionMatrix;

                //设置当前雪花粒子的组合变换矩阵和纹理
                EffectofSnow.Parameters["matWorldViewProj"].SetValue(WorldViewProjMatrix);
                switch (ParticlesofSnow[i].IndexofTexture)
                {
                    case 0:
                        EffectofSnow.Parameters["TextureMapping"].SetValue(snow1);
                        break;
                    case 1:
                        EffectofSnow.Parameters["TextureMapping"].SetValue(snow2);
                        break;
                    case 2:
                        EffectofSnow.Parameters["TextureMapping"].SetValue(snow3);
                        break;
                }

                //渲染当前雪花粒子
                EffectofSnow.CurrentTechnique = EffectofSnow.Techniques["TShaderofAlphaBlend"];
                foreach (EffectPass pass in EffectofSnow.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Game.GraphicsDevice.SetVertexBuffer(verbufofsnow);
                    Game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

                }

            }//end of for()遍历每个雪花粒子

            base.Draw(gameTime);
        }

        public Matrix CalculateProjectionMatrix()
        {
            //计算投影变换矩阵
            float viewAngle = MathHelper.PiOver4;
            float aspectRatio = Game.GraphicsDevice.Viewport.AspectRatio;
            float nearPlane = 1.0f;
            float farPlane = 10000.0f;
            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(viewAngle, aspectRatio, nearPlane, farPlane);
            return projectionMatrix;
        }

        public Matrix CalculateViewMatrix(GameTime gameTime)
        {
            //计算观察变换矩阵
            TotalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 camPosition = new Vector3(130 * (float)Math.Sin(TotalTime / 50), 1.0f, 130 * (float)Math.Cos(TotalTime / 50));
            Vector3 camTarget = new Vector3(0, 30, 0);
            Vector3 camUpVector = new Vector3(0, 1, 0);
            Matrix viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, camUpVector);
            return viewMatrix;
        }
    }

    class ParticleofSnowing
    {
        public Vector3 Position;            //位置
        public Vector3 AngelofRotation;     //沿x，y，z三个坐标轴的旋转角度
        public float DropSpeed;             //下降速度
        public float RotationSpeed;         //旋转速度
        public int IndexofTexture;          //纹理索引号

        //构造函数
        public ParticleofSnowing(Vector3 pos, Vector3 angleofrot, float speedofdrop, float speedofrot, int idoftex)
        {
            Position = pos;
            AngelofRotation = angleofrot;
            DropSpeed = speedofdrop;
            RotationSpeed = speedofrot;
            IndexofTexture = idoftex;
        }
    }
}
