using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Deffered_Loading
{
    public static class GameHelper
    {
        private static Random _rand;

        public static int RandomNext(int maxValue)
        {
            return RandomNext(0, maxValue);
        }

        public static int RandomNext(int minValue, int maxValue)
        {
            if (_rand == null) _rand = new Random();

            return _rand.Next(minValue, maxValue);
        }

        public static float RandomNext(float maxValue)
        {
            return RandomNext(0, maxValue);
        }

        public static float RandomNext(float minValue, float maxValue)
        {
            if (_rand == null) _rand = new Random();

            return (float)_rand.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
}
