using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace Rowlan.PhotoSession
{
    [Serializable]
    public class AutoFocusSettings
    {
        public static Vector2Int PresetModeOff = new Vector2Int(-1, -1);
        public static Vector2Int PresetModeCenter = new Vector2Int(1, 1);
        public static Vector2Int PresetModeAuto_4_3 = new Vector2Int(4, 3);
        public static Vector2Int PresetModeAuto_16_9 = new Vector2Int(16, 9);

        public enum Mode
        {
            /// <summary>
            /// No distance calculation
            /// </summary>
            [InspectorName("Off")]
            Off,

            /// <summary>
            /// A single ray in the center of the camera
            /// </summary>
            [InspectorName("Center")]
            Center,

            /// <summary>
            /// Multiple rays from the center of the camera
            /// </summary>
            [InspectorName("Multiple 4 x 3")]
            Auto_4_3,

            /// <summary>
            /// Multiple rays from the center of the camera
            /// </summary>
            [InspectorName("Multiple 16 x 9")]
            Auto_16_9
        }

        /// <summary>
        /// The focus mode to be used e. g. for detecting the distance for the DoF effect
        /// </summary>
        public Mode mode = Mode.Center;

        /// <summary>
        /// Whether the auto focus overlay image is visible or not
        /// </summary>
        public bool overlayVisible = true;

        /// <summary>
        /// The autofocus overlay material for visualizing and setting the focus point array
        /// </summary>
        public Material overlayMaterial = null;

        /// <summary>
        /// Maximum raycast length for the distance calculation
        /// </summary>
        public float maxRayLength = 3f;
    }

    public static class AutoFocusModeExtension
    {
        public static Vector2Int GetRays(this AutoFocusSettings.Mode mode)
        {
            switch (mode)
            {
                case AutoFocusSettings.Mode.Off: return AutoFocusSettings.PresetModeOff;
                case AutoFocusSettings.Mode.Center: return AutoFocusSettings.PresetModeCenter;
                case AutoFocusSettings.Mode.Auto_4_3: return AutoFocusSettings.PresetModeAuto_4_3;
                case AutoFocusSettings.Mode.Auto_16_9: return AutoFocusSettings.PresetModeAuto_16_9;
                default: throw new ArgumentException("Autofocus mode not defined: " + mode);
            }
        }

        /// <summary>
        /// Get an enum's attribute, e. g. <code>InspectorName</code> in order to get the text for the inspector or the UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mode"></param>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static T GetEnumAttribute<T>(this AutoFocusSettings.Mode mode, Enum enumValue) where T : Attribute
        {
            return enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<T>() as T;
        }
    }
}