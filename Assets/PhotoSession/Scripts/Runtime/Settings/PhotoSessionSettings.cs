using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Rowlan.PhotoSession.ImageResolution;

namespace Rowlan.PhotoSession
{
    [System.Serializable]
    public class PhotoSessionSettings
    {
        /// <summary>
        /// The mode the photo session is currently in.
        /// </summary>
        public enum PhotoMode
        {
            Game,
            Photo
        }

        /// <summary>
        /// The type of photo that will be stored mono and VR modes
        /// </summary>
        public enum PhotoType
        {
            Flat,
            Mono360
        }

        // Image

        public ImageResolutionType resolution = ImageResolutionType.Game;

        public PhotoType photoType = PhotoType.Flat;

        public AspectRatio aspectRatio = AspectRatio.AR_16_9;

        public Output.Format outputFormat = Output.Format.JPG;

        public bool fieldOfViewOverride = false;

        [Range(0.1f, 180f)]
        public float fieldOfView = 60f;

        // Camera Settings

        // The camera that will be used for the photo session. Usually the main camera
        public Camera photoCamera;

        // Normal camera movement speed
        public float movementSpeed = 4;

        // Camera movement speed when shift is pressed
        public float movementSpeedFast = 20;

        // Mouse look sensitivity
        public float freeLookSensitivity = 2f;

        // Normal mouse wheel zoom amount
        public float zoomSensitivity = 5;

        // Mouse wheel zoom amount when shift is pressed
        public float zoomSensitivityFast = 20;

        // Pause

        // the scaled time factor that's being used in photo mode
        [Range(0f,1f)]
        public float pauseTime = 0f;

        // Initialization

        // Keep the photo camera position and rotation the way it was in the previous session
        public bool reusePreviousCameraTransform = false;

        // Objects (e. g. Scripts) which should be disabled when photo mode is activated
        public Object[] disabledComponents;

        // User Interface

        // Optional canvas with a white stretched image. The alpha value of the image will be used to simulate a flash effect. The canvas can be hidden
        public Canvas canvas = null;

        /// <summary>
        /// The flash image which is used to simulate a flash when a photo is taken
        /// </summary>
        public Image flashImage;

        /// <summary>
        /// The display of the photo session camera. Contains e. g. focus rectangles and settings text
        /// </summary>
        public Image displayImage;

        // Optional text for displaying the image settings
        public Text imageSettingsText = null;

        // Optional text for displaying auto focus data
        public Text autoFocusText = null;

        /// <summary>
        /// The image which will be used for the composition guide overlay
        /// </summary>
        public Image compositionGuideImage = null;

        /// <summary>
        /// The composition guide which should be used as overlay
        /// </summary>
        public CompositionGuideCollection compositionGuideCollection = null;

        /// <summary>
        /// The current template index in the composition guide collection
        /// </summary>
        public int compositionGuideIndex = 0;

        /// <summary>
        /// Settings for autofocus calculation and visualization
        /// </summary>
        public AutoFocusSettings autoFocus = new AutoFocusSettings();

        /// <summary>
        /// Adjustable keyboard shortcuts
        /// </summary>
        public ShortcutSettings shortcuts = new ShortcutSettings();



        /// <summary>
        /// Attempt to autoconfigure the camera, if it is not already set.
        /// Log a warning about needing to set the camera if successfull,
        /// log an error if not successful.
        /// </summary>
        internal void ConfigureCamera()
        {
            photoCamera = Camera.main;
            if (photoCamera)
            {
                Debug.LogWarning("No camera is set in the PhotoSession settings. Automatically configured to use the main camera.");
            }
            else
            {
                Debug.LogError("No camera is set in the PhotoSession settings and there is no main camera in the scene. Either set the photoCamera or tag a camera as 'MainCamera'.");
            }
        }

        #region HDRP

        public Hdrp.DepthOfFieldSettings hdrpDepthOfFieldSettings;

        #endregion HDRP

        #region URP

        public Urp.DepthOfFieldSettings urpDepthOfFieldSettings;

        #endregion URP

        #region Legacy

        public Legacy.DepthOfFieldSettings legacyDepthOfFieldSettings;

        #endregion Legacy

    }
}