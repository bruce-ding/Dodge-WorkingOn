#region File Description
//-----------------------------------------------------------------------------
// TouchCollectionExtensions.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

// Completed
using Microsoft.Xna.Framework.Input.Touch;

namespace DodgeXaml.CommonHelper
{
    /// <summary>
    /// Provides extension methods for the TouchCollection type.
    /// </summary>
    public static class TouchCollectionExtensions
    {
        /// <summary>
        /// Determines if there are any touches on the screen.
        /// </summary>
        /// <param name="touchState">The current TouchCollection.</param>
        /// <returns>True if there are any touches in the Pressed or Moved state, false otherwise</returns>
        public static bool AnyTouch(this TouchCollection touchCollection)
        {
            foreach (TouchLocation location in touchCollection)
            {
                if (location.State == TouchLocationState.Pressed || location.State == TouchLocationState.Moved)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
