using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rowlan.PhotoSession.Urp
{
    [System.Serializable]
    public class DepthOfFieldSettings
    {
#if USING_URP
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
        /// When we hit a target, then the focus distance is set to the distance between camera and target.
        /// This value is added to the distance. So if you want to move the focus distance in front of the target, you need to specify a negative offset.
        /// </summary>
        public float focusDistanceOffset = -1f;

        /// <summary>
        /// The focal length override for the Depth of Field settings
        /// </summary>
        public float focalLength = 50f;

        /// <summary>
        /// The aperture override for the length of the Depth of Field settings
        /// </summary>
        public float aperture = 5.6f;

#endif
    }
}