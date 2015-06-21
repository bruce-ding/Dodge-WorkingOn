using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using DodgeXaml.CommonHelper.RacingGameHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DodgeXaml.CommonHelper
{
    public class LinearAnimation
    {
       
        private float fromX;
        public float FromX
        {
            private get { return fromX; }
            set 
            { 
                fromX = value;
                CurrentPosition.X = value;
            }
        }


        public float ToX;
      
        private float fromY;
        public float FromY
        {
            private get { return fromX; }
            set
            {
                fromY = value;
                CurrentPosition.Y = value;
            }
        }

        public float ToY;
        public Vector2 CurrentPosition;

        public TimeSpan XDuration;
        public TimeSpan YDuration;
        public TimeSpan AlphaDuration;

        public Texture2D TargeTexture2D;
        public Color TextureColor = Color.White;

        private float fromAlpha;

        public float FromAlpha
        {
            private get { return fromAlpha; }
            set
            {
                fromAlpha = value;
                currentAlpha = value;
            }
        }
    
        public float ToAlpha;
        private float currentAlpha;

        private bool isXCompleted;
        private bool isYCompleted;
        private bool isAlphaCompleted;
        public bool IsAllCompleted;

        public bool ShowBeforeAnimation;


        public void Update(GameTime gameTime)
        {
            if (IsAllCompleted)
            {
                return;
            }

            if (FromY < ToY)
            {
                if (CurrentPosition.Y < ToY)
                {
                    CurrentPosition.Y += (ToY - FromY) / YDuration.Seconds / 60;
                }
                else
                {
                    isYCompleted = true;
                }
            }
            else if (FromY > ToY)
            {
                if (CurrentPosition.Y > ToY)
                {
                    CurrentPosition.Y += (ToY - FromY) / YDuration.Seconds / 60;
                }
                else
                {
                    isYCompleted = true;
                }
            }

           

            if (FromX < ToX)
            {
                if (CurrentPosition.X < ToX)
                {
                    CurrentPosition.X += (ToX - FromX) / XDuration.Seconds / 60;
                }
                else
                {
                    isXCompleted = true;
                }
            }
            else if (FromX > ToX )
            {
                if (CurrentPosition.X > ToX)
                {
                    CurrentPosition.X += (ToX - FromX) / XDuration.Seconds / 60;
                }
                else
                {
                    isXCompleted = true;
                }
            }

            

            if (FromAlpha < ToAlpha)
            {
                if (currentAlpha < ToAlpha)
                {
                    currentAlpha += (ToAlpha - FromAlpha) / AlphaDuration.Seconds / 60;
                    TextureColor = ColorHelper.ApplyAlphaToColor(TextureColor, currentAlpha);
                }
                else
                {
                    isAlphaCompleted = true;
                }
            }
            else if (FromAlpha > ToAlpha)
            {
                if (currentAlpha > ToAlpha)
                {
                    currentAlpha += (ToAlpha - FromAlpha) / AlphaDuration.Seconds / 60;
                    TextureColor = ColorHelper.ApplyAlphaToColor(TextureColor, currentAlpha);
                }
                else
                {
                    isAlphaCompleted = true;
                }
            }

            IsAllCompleted = (isXCompleted && isYCompleted && IsAllCompleted);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TargeTexture2D, CurrentPosition, TextureColor);
        }
    }
}
