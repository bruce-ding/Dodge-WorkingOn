using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BloomPostprocessWin8
{
    public class BloomComponent : DrawableGameComponent
    {
        // Adjust these values to change visual appearance.
        //const float BloomThreshold = 0.25f;
        //const float BloomIntensity = 1.5f;
        //const int BlurPasses = 4;

        const float BloomThreshold = 0.25f;
        const float BloomIntensity = 0.5f;
        const int BlurPasses = 4;

        // result = source - destination
        static BlendState extractBrightColors = new BlendState
        {
            ColorSourceBlend = Blend.One,
            AlphaSourceBlend = Blend.One,

            ColorDestinationBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,

            ColorBlendFunction = BlendFunction.Subtract,
            AlphaBlendFunction = BlendFunction.Subtract,
        };


        // result = source + destination
        static BlendState additiveBlur = new BlendState
        {
            ColorSourceBlend = Blend.One,
            AlphaSourceBlend = Blend.One,

            ColorDestinationBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
        };


        // result = source + (destination * (1 - source))
        static BlendState combineFinalResult = new BlendState
        {
            ColorSourceBlend = Blend.One,
            AlphaSourceBlend = Blend.One,

            ColorDestinationBlend = Blend.InverseSourceColor,
            AlphaDestinationBlend = Blend.InverseSourceColor,
        };


        SpriteBatch spriteBatch;

        RenderTarget2D scene;
        RenderTarget2D halfSize;
        RenderTarget2D quarterSize;
        RenderTarget2D quarterSize2;


        public BloomComponent(Game game)
            : base(game)
        { }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            PresentationParameters pp = GraphicsDevice.PresentationParameters;

            int w = pp.BackBufferWidth;
            int h = pp.BackBufferHeight;

            scene = new RenderTarget2D(GraphicsDevice, w, h, false, pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
            halfSize = new RenderTarget2D(GraphicsDevice, w / 2, h / 2, false, pp.BackBufferFormat, DepthFormat.None);
            quarterSize = new RenderTarget2D(GraphicsDevice, w / 4, h / 4, false, pp.BackBufferFormat, DepthFormat.None);
            quarterSize2 = new RenderTarget2D(GraphicsDevice, w / 4, h / 4, false, pp.BackBufferFormat, DepthFormat.None);
        }


        public void BeginDraw()
        {
            if (Visible)
            {
                GraphicsDevice.SetRenderTarget(scene);
            }
        }


        public override void Draw(GameTime gameTime)
        {
            // Shrink to half size.
            GraphicsDevice.SetRenderTarget(halfSize);
            DrawSprite(scene, BlendState.Opaque);

            // Shrink again to quarter size, at the same time applying the threshold subtraction.
            GraphicsDevice.SetRenderTarget(quarterSize);
            GraphicsDevice.Clear(new Color(BloomThreshold, BloomThreshold, BloomThreshold));
            DrawSprite(halfSize, extractBrightColors);
            
            // Kawase blur filter (see http://developer.amd.com/media/gpu_assets/Oat-ScenePostprocessing.pdf)
            for (int i = 0; i < BlurPasses; i++)
            {
                GraphicsDevice.SetRenderTarget(quarterSize2);
                GraphicsDevice.Clear(Color.Black);

                int w = quarterSize.Width;
                int h = quarterSize.Height;

                float brightness = 0.25f;

                // On the first pass, scale brightness to restore full range after the threshold subtraction.
                if (i == 0)
                    brightness /= (1 - BloomThreshold);

                // On the final pass, apply tweakable intensity adjustment.
                if (i == BlurPasses - 1)
                    brightness *= BloomIntensity;

                Color tint = new Color(brightness, brightness, brightness);

                spriteBatch.Begin(0, additiveBlur);

                spriteBatch.Draw(quarterSize, new Vector2(0.5f, 0.5f), new Rectangle(i + 1, i + 1, w, h), tint);
                spriteBatch.Draw(quarterSize, new Vector2(0.5f, 0.5f), new Rectangle(-i, i + 1, w, h), tint);
                spriteBatch.Draw(quarterSize, new Vector2(0.5f, 0.5f), new Rectangle(i + 1, -i, w, h), tint);
                spriteBatch.Draw(quarterSize, new Vector2(0.5f, 0.5f), new Rectangle(-i, -i, w, h), tint);

                spriteBatch.End();

                Swap(ref quarterSize, ref quarterSize2);
            }

            // Combine the original scene and bloom images.
            GraphicsDevice.SetRenderTarget(null);
            DrawSprite(scene, BlendState.Opaque);
            DrawSprite(quarterSize, combineFinalResult);
        }


        void DrawSprite(Texture2D source, BlendState blendState)
        {
            spriteBatch.Begin(0, blendState);
            spriteBatch.Draw(source, GraphicsDevice.Viewport.Bounds, Color.White);
            spriteBatch.End();
        }


        static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }
    }
}