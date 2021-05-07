using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.PhotoSession
{
    [Serializable]
    public class AutoFocusInput
    {
        /// <summary>
        /// Whether the camera bounds are considered for raycast coordinates or not. Ususally you don't want them since the data there is rather irrelevant.
        /// </summary>
        public bool skipBounds = true;

        // shader has 144 predefined for the array, so max is 16x9=144
        [Range(1, 16)]
        public int focusRaysX = 4;

        // shader has 144 predefined for the array, so max is 16x9=144
        [Range(1, 9)]
        public int focusRaysY = 3;

        /// <summary>
        /// The maximum ray length is used to calculate the opacity of the focus hit rectangles.
        /// </summary>
        public float maxRayLength = 3;
    }
}