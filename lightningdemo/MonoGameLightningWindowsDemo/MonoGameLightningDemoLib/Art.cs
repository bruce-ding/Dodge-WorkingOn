using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoGameLightningDemoLib
{
	public static class Art
	{
		public static Texture2D LightningSegment, HalfCircle, Pixel;
	    public static SoundEffectInstance LightingSound;

		public static void Load(ContentManager content)
		{
			LightningSegment = content.Load<Texture2D>("Lightning Segment");
			HalfCircle = content.Load<Texture2D>("Half Circle");
			Pixel = content.Load<Texture2D>("Pixel");
		    LightingSound = content.Load<SoundEffect>("Audio/lighting").CreateInstance();
		}
	}
}
