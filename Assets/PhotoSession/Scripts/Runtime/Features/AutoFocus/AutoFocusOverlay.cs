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
        private readonly int maxArrayLength = 144;

        /// <summary>
        /// The material with the shader that will be used to display the focus rectangles
        /// </summary>
        public Material material;

        /// <summary>
        /// Visualize raycast as gizmo
        /// </summary>
        public bool debug = false;

        public AutoFocusInput autoFocusInput = new AutoFocusInput();

        /// <summary>
        /// The output data which result from the autofocus raycasts and calculations
        /// </summary>
        private AutoFocusData autoFocusData = new AutoFocusData();

        void Start()
        {
            // initialize the float array
            material.SetInt("_ArrayLength", maxArrayLength);
            material.SetFloatArray("_PositionX", new float[maxArrayLength]);
            material.SetFloatArray("_PositionY", new float[maxArrayLength]);
            material.SetFloatArray("_Distance", new float[maxArrayLength]);
            material.SetFloat("_MaxHitDistance", autoFocusData.maxRayLength);
        }

        void Update()
        {
            AutoFocusCalculation.UpdateOutputData( this.autoFocusInput, this.autoFocusData);

            int arrayLength = this.autoFocusData.screenPoints.Count;
            float[] positionX = this.autoFocusData.screenPoints.ConvertAll<float>(x => x.x).ToArray();
            float[] positionY = this.autoFocusData.screenPoints.ConvertAll<float>(x => x.y).ToArray();
            float[] distance = this.autoFocusData.screenPoints.ConvertAll<float>(x => x.z).ToArray();

            material.SetInt("_ArrayLength", arrayLength);
            material.SetFloatArray("_PositionX", positionX);
            material.SetFloatArray("_PositionY", positionY);
            material.SetFloatArray("_Distance", distance);
            material.SetFloat("_MaxHitDistance", autoFocusData.maxRayLength);

        }


    }
}
