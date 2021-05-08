using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Rowlan.PhotoSession
{
    /// <summary>
    /// The output data which result from the autofocus raycasts and calculations
    /// </summary>
    public class AutoFocusData
    {
        /// <summary>
        /// Whether a hit target was found or not
        /// </summary>
        public bool hasTarget = false;

        /// <summary>
        /// The raycast points. All of them.
        /// Raycast targets which are inside the maximum hit distance have z value -1
        /// </summary>
        public List<Vector3> screenPoints = new List<Vector3>();

        /// <summary>
        /// The minimum focus distance if <see cref="hasTarget"/> is true, -1 otherwise
        /// </summary>
        public float minDistance = -1;

        /// <summary>
        /// The maximum focus distance if <see cref="hasTarget"/> is true, -1 otherwise
        /// </summary>
        public float maxDistance = -1;

        /// <summary>
        /// The maximum ray length is used to calculate the opacity of the focus hit rectangles.
        /// This is driven externally.
        /// </summary>
        public float maxRayLength = -1;

        /// <summary>
        /// Reset the data to their default values
        /// </summary>
        public void Reset()
        {
            hasTarget = false;
            screenPoints.Clear();
            minDistance = -1;
            maxDistance = -1;
        }

        /// <summary>
        /// Calculate the autofocus data like minimum and maximum distance
        /// </summary>
        public void Calculate()
        {
            minDistance = hasTarget ? screenPoints.Min(v => v.z) : -1;
            maxDistance = hasTarget ? screenPoints.Max(v => v.z) : -1;
        }

        public bool IsTargetInRange()
        {
            return hasTarget && minDistance != -1 && minDistance < maxRayLength;
		}
    }
}