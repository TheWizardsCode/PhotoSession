using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.PhotoSession
{
    public class AutoFocusOverlayModule : IPhotoSessionModule
    {
        /// <summary>
        /// The maximum array size of position and distance in the shader
        /// </summary>
        private static readonly int maxArrayLength = 144;
        private static readonly float[] emptyArray = new float[maxArrayLength];

        /// <summary>
        /// The material with the shader that will be used to display the focus rectangles
        /// </summary>
        private Material material;

        private PhotoSession photoSession;

        public void Start(PhotoSession photoSession)
		{
            this.photoSession = photoSession;
            this.material = photoSession.settings.autoFocus.overlayMaterial;

            // initialize the float array
            material.SetInt("_ArrayLength", maxArrayLength);
            material.SetFloatArray("_PositionX", emptyArray);
            material.SetFloatArray("_PositionY", emptyArray);
            material.SetFloatArray("_Distance", emptyArray);
            material.SetFloat("_MaxHitDistance", -1);
        }

		void IPhotoSessionModule.Update()
		{
            // updated externally
            // AutoFocusCalculation.UpdateOutputData( this.autoFocusInput, this.autoFocusData);

            int arrayLength = photoSession.settings.autoFocus.overlayVisible ? photoSession.autoFocusData.screenPoints.Count : 0;

            material.SetInt("_ArrayLength", arrayLength);

            float[] positionX = photoSession.autoFocusData.screenPoints.ConvertAll(x => x.x).ToArray();
            float[] positionY = photoSession.autoFocusData.screenPoints.ConvertAll(x => x.y).ToArray();
            float[] distance = photoSession.autoFocusData.screenPoints.ConvertAll(x => x.z).ToArray();

            if (positionX.Length > 0) material.SetFloatArray("_PositionX", positionX); // set the array; zero-sized array isn't allowed generally in the shader
            if (positionY.Length > 0) material.SetFloatArray("_PositionY", positionY); // set the array; zero-sized array isn't allowed generally in the shader
            if (distance.Length > 0) material.SetFloatArray("_Distance", distance); // set the array; zero-sized array isn't allowed generally in the shader

            material.SetFloat("_MaxHitDistance", photoSession.autoFocusData.maxRayLength);
        }

		public void OnEnable()
		{
			// nothing to do
		}

		public void OnDisable()
		{
            // nothing to do
        }

        public void OnDrawGizmos()
		{
            // nothing to do
        }
    }
}
