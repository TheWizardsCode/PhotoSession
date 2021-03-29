using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rowlan.PhotoSession.Hdrp
{
    [System.Serializable]
    public class DepthOfFieldSettings
    {
#if USING_HDRP
        /// <summary>
        /// Whether the feature is enabled or not at runtime
        /// </summary>
        public bool featureEnabled;

        /// <summary>
        /// The volume which contains the depth of field component.
        /// It can be disabled in the project. 
        /// If this DoF feature is enabled, then 
        /// + the gameobject is activated
        /// + the volume's depth of field script of the gameobject's volume is searched and activated.
        /// </summary>
        public Volume volume;

        /// <summary>
        /// The maximum focus distance for the raycast upon which we determine whether to activate the DoF mode or not
        /// </summary>
        public float maxFocusDistance = 10f;

        /// <summary>
        /// When we hit a target, then the near focus end is set to the distance between camera and target.
        /// This value is added to the distance. So if you want to move the near focus end in front of the target, you need to specify a negative offset.
        /// </summary>
        public float hitDistanceNearFocusEndOffset = -0.5f;

        /// <summary>
        /// When we hit a target, then the far focus start is set to the distance between camera and target.
        /// This value is added to the far focus start.
        /// </summary>
        public float hitDistanceFarFocusStartOffset = 1f;

        /// <summary>
        /// When we hit a target, then the far focus start is set to the distance between camera and target.
        /// This value influences the far focus end value which is the far focus start plus this offset.
        /// </summary>
        public float hitDistanceFarFocusEndOffset = 10f;
#endif
    }
}