using System.Collections;
using UnityEngine;
using static Rowlan.PhotoSession.ImageResolution;
using static Rowlan.PhotoSession.PhotoSessionSettings;

namespace Rowlan.PhotoSession
{
    /// <summary>
    /// Pause the game and allow free look movement of the camera in order to capture screenshots
    /// </summary>
    public class PhotoSession : MonoBehaviour
    {
        /// <summary>
        /// The settings for this photo session
        /// </summary>
        [HideInInspector]
        public PhotoSessionSettings settings = new PhotoSessionSettings();

        /// <summary>
        /// The current mode, either game or photo
        /// </summary>
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
            cameraFlash = new CameraFlash(settings.canvas);
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
            playerCameraState.Save(settings.photoCamera.transform);

            // disable components (eg scripts)
            foreach (Object script in settings.disabledComponents)
            {

                if (script is Behaviour behaviour)
                {
                    behaviour.enabled = false;
                }
                else if (script is GameObject go)
                {
                    go.SetActive( false);
                }
            }

            // detach from parent
            settings.photoCamera.transform.parent = null;

            // in case of reuse of the previous photo session we restore the previous camera transform
            if (settings.reusePreviousCameraTransform && previousPhotoCameraInitialized)
            {
                // reuse previous camera transform
                previousPhotoCameraState.Restore(settings.photoCamera.transform);
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
            previousPhotoCameraState.Save(settings.photoCamera.transform);
            previousPhotoCameraInitialized = true;

            // restore the original camera's transform
            playerCameraState.Restore(settings.photoCamera.transform);

            // re-enable components
            foreach (Object script in settings.disabledComponents)
            {
                if (script is Behaviour behaviour)
                {
                    behaviour.enabled = true;
                }
                else if (script is GameObject go)
                {
                    go.SetActive(true);
                }
            }
        }

        void Update()
        {
            // check input key and toggle game <> photo mode
            if (Input.GetKeyDown(settings.toggleKey))
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

                Transform cameraTransform = settings.photoCamera.transform;

                // determine speed
                var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                var movementSpeed = fastMode ? this.settings.movementSpeedFast : this.settings.movementSpeed;

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

                float newRotationX = cameraTransform.localEulerAngles.y + Input.GetAxis("Mouse X") * settings.freeLookSensitivity;
                float newRotationY = cameraTransform.localEulerAngles.x - Input.GetAxis("Mouse Y") * settings.freeLookSensitivity;

                cameraTransform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);

                float axis = Input.GetAxis("Mouse ScrollWheel");
                if (axis != 0)
                {
                    var zoomSensitivity = fastMode ? this.settings.zoomSensitivityFast : this.settings.zoomSensitivity;

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

            if (!settings.canvas)
                return;

            bool canvasActive = photoMode == PhotoMode.Photo;

            settings.canvas.gameObject.SetActive(canvasActive);
        }

        /// <summary>
        /// Capture a screenshot and simulate a flashlight effect
        /// </summary>
        IEnumerator CaptureScreenshot() 
        {
            // stop flashlight, we don't want anything of the flashlight in the screenshot
            cameraFlash.StopCameraFlash(this);

            // hide the canvas, we don't want anything of it (eg photo mode text) in the screenshot
            SetCanvasVisible(false);

            // capturing must only happen when everything was drawn (including the flashlight overlay removed)
            yield return new WaitForEndOfFrame();

			// effectively save the screenshot
			screenshot.Capture(settings.photoCamera, settings.resolution.GetImageResolution( settings.aspectRatio).Width, settings.resolution.GetImageResolution( settings.aspectRatio).Height, settings.outputFormat);

            // show canvas for flashlight effect
            SetCanvasVisible(true);

            // start flashlight effect
            cameraFlash.StartCameraFlash(this);

		}

        private void SetCanvasVisible( bool visible) {
            
            if (!settings.canvas)
                return;

            settings.canvas.gameObject.SetActive(visible);
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