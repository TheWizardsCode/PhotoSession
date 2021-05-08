using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.PhotoSession
{
    public class AutoFocusOverlay : MonoBehaviour
    {
        /// <summary>
        /// The maximum array size of position and distance in the shader
        /// </summary>
        private static readonly int maxArrayLength = 144;
        private static readonly float[] emptyArray = new float[maxArrayLength];

        /// <summary>
        /// The material with the shader that will be used to display the focus rectangles
        /// </summary>
        public Material material;

        /// <summary>
        /// Visualize raycast as gizmo
        /// </summary>
        public bool debug = false;

        public AutoFocusInput autoFocusInput { get; set; } = new AutoFocusInput();

        /// <summary>
        /// The output data which result from the autofocus raycasts and calculations
        /// </summary>
        public AutoFocusData autoFocusData { get; set; } = new AutoFocusData();

        public bool visible = true;

        void Start()
        {
            // initialize the float array
            material.SetInt("_ArrayLength", maxArrayLength);
            material.SetFloatArray("_PositionX", emptyArray);
            material.SetFloatArray("_PositionY", emptyArray);
            material.SetFloatArray("_Distance", emptyArray);
            material.SetFloat("_MaxHitDistance", autoFocusData.maxRayLength);
        }

        void Update()
        {
            // updated externally
            // AutoFocusCalculation.UpdateOutputData( this.autoFocusInput, this.autoFocusData);

            int arrayLength = visible ? this.autoFocusData.screenPoints.Count : 0;
            
            material.SetInt("_ArrayLength", arrayLength);

            float[] positionX = this.autoFocusData.screenPoints.ConvertAll(x => x.x).ToArray();
            float[] positionY = this.autoFocusData.screenPoints.ConvertAll(x => x.y).ToArray();
            float[] distance = this.autoFocusData.screenPoints.ConvertAll(x => x.z).ToArray();

            if (positionX.Length > 0) material.SetFloatArray("_PositionX", positionX); // set the array; zero-sized array isn't allowed generally in the shader
            if (positionY.Length > 0) material.SetFloatArray("_PositionY", positionY); // set the array; zero-sized array isn't allowed generally in the shader
            if (distance.Length > 0) material.SetFloatArray("_Distance", distance); // set the array; zero-sized array isn't allowed generally in the shader

            material.SetFloat("_MaxHitDistance", autoFocusData.maxRayLength);

        }

    }
}
