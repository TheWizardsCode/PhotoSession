using System.Collections;
using UnityEngine;
using static Rowlan.PhotoSession.ImageResolution;

namespace Rowlan.PhotoSession
{
    /// <summary>
    /// Pause the game and allow free look movement of the camera in order to capture screenshots
    /// </summary>
    public class PhotoSession : MonoBehaviour
    {
        /// <summary>
        /// The mode the photo session is currently in.
        /// </summary>
        public enum PhotoMode
        {
            Game,
            Photo
        }

        [Header("Image")]

        public ImageResolutionType resolution = ImageResolutionType.Game;

        public Output.Format outputFormat = Output.Format.JPG;

        [Header("Input")]

        [Tooltip("Input key which toggles the photo mode")]
        public KeyCode toggleKey = KeyCode.F12;

        [Header("Camera Settings")]

        [Tooltip("The camera that will be used for the photo session. Usually the main camera")]
        public Camera photoCamera;

        [Tooltip("Normal camera movement speed")]
        public float movementSpeed = 4;

        [Tooltip("Camera movement speed when shift is pressed")]
        public float movementSpeedFast = 20;

        [Tooltip("Mouse look sensitivity")]
        public float freeLookSensitivity = 2f;

        [Tooltip("Normal mouse wheel zoom amount")]
        public float zoomSensitivity = 5;

        [Tooltip("Mouse wheel zoom amount when shift is pressed")]
        public float zoomSensitivityFast = 20;

        [Header("Initialization")]

        [Tooltip("Keep the photo camera position and rotation the way it was in the previous session")]
        public bool reusePreviousCameraTransform = false;

        [Space]

        [Tooltip("Objects (e. g. Scripts) which should be disabled when photo mode is activated")]
        public Object[] disabledComponents;

        [Header("User Interface")]

        [Tooltip("An optional canvas with a white stretched image. The alpha value of the image will be used to simulate a flash effect. The canvas can be hidden")]
        public Canvas canvas = null;

        private PhotoMode photoMode = PhotoMode.Game;

        /// <summary>
        /// Used for storing the player's camera which got hijacked for the photo session
        /// </summary>
        private TransformState playerCameraState = new TransformState();

        /// <summary>
        /// Save and restore the cursor visibility and lock mode
        /// </summary>
        private CursorState cursorState = new CursorState();

        /// <summary>
        /// Indicator whether the previous photo camera transform can be used or not
        /// </summary>
        private bool previousPhotoCameraInitialized = false;

        /// <summary>
        /// Used for saving and restoring the previous photo camera transform
        /// </summary>
        private TransformState previousPhotoCameraState = new TransformState();

        /// <summary>
        /// Utility for capturing screenshots
        /// </summary>
        private Screenshot screenshot = new Screenshot();

        /// <summary>
        /// A <see cref="CameraFlash"/> scene gameobject which will be used to simulate a flash.
        /// </summary>
        private CameraFlash cameraFlash = null;

        /// <summary>
        /// Seconds for which the photo mode input will be delayed.
        /// This is used so that eg when you are in running mode with shift the shift doesn't get applied to the photo mode input and you move away from the current camera position.
        /// </summary>
        private float delayedPhotoModeInputTime = 0.3f;

        /// <summary>
        /// Whether input controls are active or not in photo mode
        /// </summary>
        private bool photoModeInputActive = false;

        /// <summary>
        /// Reference for the delayed input coroutine which allows us to stop it
        /// </summary>
        private Coroutine delayedPhotoModeInputCoroutine = null;

        void Awake()
        {
            screenshot.SetupPath();
        }

        void Start()
        {
            cameraFlash = new CameraFlash(canvas);
        }

        void PauseGame()
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }

        void ResumeGame()
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
        }

        void EnablePhotoCamera()
        {
            delayedPhotoModeInputCoroutine = StartCoroutine(DelayedPhotoModeInput());

            // save the cursor's state, hide and lock it
            cursorState.Save();
            cursorState.Lock();

            // player photo camera state so that it can be restored after we leave the session
            playerCameraState.Save( photoCamera.transform);

            // disable components (eg scripts)
            foreach (Object script in disabledComponents)
            {
                if (script is Behaviour behaviour)
                    behaviour.enabled = false;
            }

            // detach from parent
            photoCamera.transform.parent = null;

            // in case of reuse of the previous photo session we restore the previous camera transform
            if (reusePreviousCameraTransform && previousPhotoCameraInitialized)
            {
                // reuse previous camera transform
                previousPhotoCameraState.Restore(photoCamera.transform);
            }
        }

        /// <summary>
        /// Delay the input controls for photo mode
        /// </summary>
        /// <returns></returns>
        IEnumerator DelayedPhotoModeInput() {

            // need to use unscaled time
            yield return new WaitForSecondsRealtime(delayedPhotoModeInputTime);

            photoModeInputActive = true;

        }

        void DisablePhotoCamera()
        {
            if (delayedPhotoModeInputCoroutine != null)
            {
                StopCoroutine(delayedPhotoModeInputCoroutine);
            }

            photoModeInputActive = false;

            /// restore the previous cursor state
            cursorState.Restore();

            // save current transform of photoshoot camera
            previousPhotoCameraState.Save(photoCamera.transform);
            previousPhotoCameraInitialized = true;

            // restore the original camera's transform
            playerCameraState.Restore(photoCamera.transform);

            // re-enable components
            foreach (Object script in disabledComponents)
            {
                if (script is Behaviour behaviour)
                    behaviour.enabled = true;
            }
        }

        void Update()
        {
            // check input key and toggle game <> photo mode
            if (Input.GetKeyDown(toggleKey))
            {
                // toggle mode
                photoMode = photoMode == PhotoMode.Game ? PhotoMode.Photo : PhotoMode.Game;

                // game mode: disable photo camera and resume game
                if (photoMode == PhotoMode.Game)
                {
                    DisablePhotoCamera();

                    ResumeGame();

                }
                // photo mode: pause game and enable photo camera
                else if (photoMode == PhotoMode.Photo)
                {
                    PauseGame();

                    EnablePhotoCamera();
                }
            }

            // photo canvas visibility
            UpdateCanvasVisibility();

            // disable photo mode input for the specified time
            if (!photoModeInputActive)
                return;

            // apply photo mode logic
            if (photoMode == PhotoMode.Photo)
            {
                // we are in pause mode => we need to use Time.unscaledDeltaTime instead of Time.deltaTime or otherwise the camera couldn't move
                float time = Time.unscaledDeltaTime;

                Transform cameraTransform = photoCamera.transform;

                // determine speed
                var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                var movementSpeed = fastMode ? this.movementSpeedFast : this.movementSpeed;

				#region Keyboard

				if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    cameraTransform.position += -cameraTransform.right * movementSpeed * time;
                }

                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    cameraTransform.position += cameraTransform.right * movementSpeed * time;
                }

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    cameraTransform.position += cameraTransform.forward * movementSpeed * time;
                }

                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    cameraTransform.position += -cameraTransform.forward * movementSpeed * time;
                }

                if (Input.GetKey(KeyCode.E))
                {
                    cameraTransform.position += cameraTransform.up * movementSpeed * time;
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    cameraTransform.position += -cameraTransform.up * movementSpeed * time;
                }

                #endregion Keyboard

                #region Mouse Movement

                float newRotationX = cameraTransform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
                float newRotationY = cameraTransform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;

                cameraTransform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);

                float axis = Input.GetAxis("Mouse ScrollWheel");
                if (axis != 0)
                {
                    var zoomSensitivity = fastMode ? this.zoomSensitivityFast : this.zoomSensitivity;

                    cameraTransform.position += cameraTransform.forward * axis * zoomSensitivity;
                }

                #endregion Mouse Movement

                #region Screenshot

                // left mouse button takes screenshots
                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine( CaptureScreenshot());
                }

                #endregion Screenshot
                
            }

        }

        /// <summary>
        /// Show photo mode indicator in photo mode, otherwise hide the canvas
        /// </summary>
        void UpdateCanvasVisibility() {

            if (!canvas)
                return;

            bool canvasActive = photoMode == PhotoMode.Photo;

            canvas.gameObject.SetActive(canvasActive);
        }

        /// <summary>
        /// Capture a screenshot and simulate a flashlight effect
        /// </summary>
        IEnumerator CaptureScreenshot() 
        {
            // stop flashlight, we don't want anything of the flashlight in the screenshot
            cameraFlash.StopCameraFlash(this);

            // hide the canvas, we don't want anything of it (eg photo mode text) in the screenshot
            canvas.gameObject.SetActive(false);

            // capturing must only happen when everything was drawn (including the flashlight overlay removed)
            yield return new WaitForEndOfFrame();

			// effectively save the screenshot
			screenshot.Capture( photoCamera, resolution.GetImageResolution().Width, resolution.GetImageResolution().Height, outputFormat);

            // show canvas for flashlight effect
            canvas.gameObject.SetActive(true);

            // start flashlight effect
            cameraFlash.StartCameraFlash(this);

		}

		/// <summary>
		/// Container for the transform data of the camera
		/// </summary>
		private class TransformState
        {

            private Transform parent = null;
            private Vector3 position;
            private Quaternion rotation;

            public void Save(Transform transform)
            {
                parent = transform.parent;
                position = transform.position;
                rotation = transform.rotation;
            }

            public void Restore(Transform transform)
            {
                transform.parent = parent;
                transform.position = position;
                transform.rotation = rotation;
            }

        }

        /// <summary>
        /// Container for the cursor visibility and lock state
        /// </summary>
        private class CursorState
        {

            private bool visible;
            private CursorLockMode lockMode;

            public void Save()
            {
                visible = Cursor.visible;
                lockMode = Cursor.lockState;
            }

            public void Restore()
            {
                Cursor.visible = visible;
                Cursor.lockState = lockMode;
            }

            public void Lock()
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

        }
	}
}