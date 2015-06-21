using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodgeXaml.Utilities
{
    public static class Texture2DFactory
    {
        public static Texture2D New(GraphicsDevice graphicsDevice, int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height, false, SurfaceFormat.Color);

            Color[] colorData = new Color[width * height];
            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = color;
            }
            texture.SetData(colorData);

            return texture;
        }
    }
}
