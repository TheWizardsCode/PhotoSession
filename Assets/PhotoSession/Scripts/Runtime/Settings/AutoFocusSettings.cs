using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rowlan.PhotoSession
{
    [Serializable]
    public class AutoFocusSettings
    {
        public enum Mode
        {
            /// <summary>
            /// No distance calculation
            /// </summary>
            Off,

            /// <summary>
            /// A single ray in the center of the camera
            /// </summary>
            Center,

            /// <summary>
            /// Multiple rays from the center of the camera
            /// </summary>
            Auto_16_9
        }

        /// <summary>
        /// The focus mode to be used e. g. for detecting the distance for the DoF effect
        /// </summary>
        public Mode mode = Mode.Center;

        /// <summary>
        /// The image on which the focus rectangle will be painted
        /// </summary>
        public Image image = null;
    }
}