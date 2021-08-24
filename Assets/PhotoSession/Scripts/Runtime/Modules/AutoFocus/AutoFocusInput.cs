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

        /// <summary>
        /// The number of focus rays in x and y dimension
        /// </summary>
        public Vector2Int focusRays = AutoFocusSettings.PresetModeCenter;

        /// <summary>
        /// The maximum ray length is used to calculate the opacity of the focus hit rectangles.
        /// </summary>
        public float maxRayLength = 3;

        /// <summary>
        /// The layer mask to be used for the raycast
        /// </summary>
        public LayerMask layerMask = (int) LayerUtils.LayerIndex.Everything;
    }
}