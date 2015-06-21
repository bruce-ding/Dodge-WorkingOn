﻿//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DodgeXaml
{
	static class EnemySpawner
	{
		static Random rand = new Random();
		static float inverseSpawnChance = 90;
		static float inverseBlackHoleChance = 600;

		public static void Update()
		{
			if ( EntityManager.Count < 200)
			{
				if (rand.Next((int)inverseSpawnChance) == 0)
					EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));

				if (rand.Next((int)inverseSpawnChance) == 0)
					EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));

                //if (EntityManager.BlackHoleCount < 2 && rand.Next((int)inverseBlackHoleChance) == 0)
                //    EntityManager.Add(new BlackHole(GetSpawnPosition()));
			}
			
			// slowly increase the spawn rate as time progresses
			if (inverseSpawnChance > 30)
				inverseSpawnChance -= 0.005f;
		}

		private static Vector2 GetSpawnPosition()
		{
			Vector2 pos;
			do
			{
				pos = new Vector2(rand.Next((int)Game1.Current.ScreenSize.X), rand.Next((int)Game1.Current.ScreenSize.Y));
			} 
			while (Vector2.DistanceSquared(pos, SpriteManager.Current.player.CenterPosition) < 300 * 300);

			return pos;
		}

		public static void Reset()
		{
			inverseSpawnChance = 90;
		}
	}
}
