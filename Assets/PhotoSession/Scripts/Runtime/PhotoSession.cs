using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
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
        private CameraState playerCameraState = new CameraState();

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
        private CameraState previousPhotoCameraState = new CameraState();

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

        private List<IPhotoSessionModule> modules = new List<IPhotoSessionModule>();

        public AutoFocusInput autoFocusInput = new AutoFocusInput();
        public AutoFocusData autoFocusData = new AutoFocusData();
        private Vector3 manualFocusPosition = Vector3.zero;

        /// <summary>
        /// Formatter for the numeric display data
        /// </summary>
        private NumberFormatInfo numberFormatInfo = new CultureInfo("en-US", false).NumberFormat;

        /// <summary>
        /// Original state of blacklisted gameobjects.
        /// Only because it's blacklisted doesn't mean it was active.
        /// </summary>
        private Dictionary<Object, bool> blacklistActiveState = new Dictionary<Object, bool>();

        void Awake()
        {
            screenshot.SetupPath();
        }

        void Start()
        {
            cameraFlash = new CameraFlash(settings.flashImage);

            RegisterModules();
            StartModules();

            UpdateAutoFocusSetup();
            UpdateTextComponents();
            UpdateCompositionGuideOverlay();

        }

        private void RegisterModules() {

            modules.Add(new AutoFocusOverlayModule());

#if USING_HDRP                
            modules.Add(new Hdrp.DepthOfFieldModule());
#endif
#if USING_URP
            modules.Add(new Urp.DepthOfFieldModule());
#endif
#if USING_LEGACY
            modules.Add(new Legacy.DepthOfFieldModule());
#endif
        }

        void PauseGame()
        {
            Time.timeScale = settings.pauseTime;
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

            // player photo camera state so that it can be restored after we leave the session
            playerCameraState.Save(settings.photoCamera.transform);

            // clear original state registry
            blacklistActiveState.Clear();

            // disable components (eg scripts)
            foreach (Object script in settings.disabledComponents)
            {

                bool wasActive = false;

                if (script is Behaviour behaviour)
                {
                    wasActive = behaviour.enabled;
                    behaviour.enabled = false;
                }
                else if (script is GameObject go)
                {
                    wasActive = go.activeInHierarchy;
                    go.SetActive( false);
                }

                blacklistActiveState.Add(script, wasActive);
            }

            // detach from parent
            settings.photoCamera.transform.parent = null;

            // in case of reuse of the previous photo session we restore the previous camera transform
            if (settings.reusePreviousCameraTransform && previousPhotoCameraInitialized)
            {
                // reuse previous camera transform
                previousPhotoCameraState.Restore(settings.photoCamera.transform);
            }

            // cursor handling: save initial state and unlock photo session cursor mode
            cursorState.Save();
            cursorState.Unlock();

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

            // save current transform of photoshoot camera
            previousPhotoCameraState.Save(settings.photoCamera.transform);
            previousPhotoCameraInitialized = true;

            // restore the original camera's transform
            playerCameraState.Restore(settings.photoCamera.transform);

            // re-enable components
            foreach (Object script in settings.disabledComponents)
            {
                bool wasActive;
                blacklistActiveState.TryGetValue(script, out wasActive);

                if (script is Behaviour behaviour)
                {
                    behaviour.enabled = wasActive;
                }
                else if (script is GameObject go)
                {
                    go.SetActive(wasActive);
                }
            }

            // cursor handling
            cursorState.Restore();

        }

        void Update()
        {
            // check input key and toggle game <> photo mode
            if (settings.shortcuts.IsTogglePhotoSession())
            {
                // toggle mode
                photoMode = photoMode == PhotoMode.Game ? PhotoMode.Photo : PhotoMode.Game;

                // game mode: disable photo camera and resume game
                if (photoMode == PhotoMode.Game)
                {
                    DisableModules();

                    DisablePhotoCamera();

                    ResumeGame();

                }
                // photo mode: pause game and enable photo camera
                else if (photoMode == PhotoMode.Photo)
                {
                    PauseGame();

                    EnablePhotoCamera();

                    EnableModules();
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
                if (settings.shortcuts.IsMouseRightDown())
                {
                    cursorState.Lock();
                }

                if (settings.shortcuts.IsMouseRightUp())
                {
                    cursorState.Unlock();
                }

                if (settings.shortcuts.IsMouseRightPressed())
                {
                    // we are in pause mode => we need to use Time.unscaledDeltaTime instead of Time.deltaTime or otherwise the camera couldn't move
                    float time = Time.unscaledDeltaTime;

                    Transform cameraTransform = settings.photoCamera.transform;

                    // determine speed
                    var fastMode = settings.shortcuts.IsMoveFast();
                    var movementSpeed = fastMode ? this.settings.movementSpeedFast : this.settings.movementSpeed;

                    #region Keyboard




                    if (settings.shortcuts.IsMoveLeft())
                    {
                        cameraTransform.position += -cameraTransform.right * movementSpeed * time;
                    }

                    if (settings.shortcuts.IsMoveRight())
                    {
                        cameraTransform.position += cameraTransform.right * movementSpeed * time;
                    }

                    if (settings.shortcuts.IsMoveForward())
                    {
                        cameraTransform.position += cameraTransform.forward * movementSpeed * time;
                    }

                    if (settings.shortcuts.IsMoveBack())
                    {
                        cameraTransform.position += -cameraTransform.forward * movementSpeed * time;
                    }

                    if (settings.shortcuts.IsMoveUp())
                    {
                        cameraTransform.position += cameraTransform.up * movementSpeed * time;
                    }

                    if (settings.shortcuts.IsMoveDown())
                    {
                        cameraTransform.position += -cameraTransform.up * movementSpeed * time;
                    }

                    #endregion Keyboard

                    #region Mouse Movement

                    float newRotationX = cameraTransform.localEulerAngles.y +
                                         settings.shortcuts.GetMouseAxisX() * settings.freeLookSensitivity;
                    float newRotationY = cameraTransform.localEulerAngles.x -
                                         settings.shortcuts.GetMouseAxisY() * settings.freeLookSensitivity;

                    cameraTransform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);

                    float axis = settings.shortcuts.GetMouseAxisWheel();
                    if (axis != 0)
                    {
                        var zoomSensitivity =
                            fastMode ? this.settings.zoomSensitivityFast : this.settings.zoomSensitivity;

                        cameraTransform.position += cameraTransform.forward * axis * zoomSensitivity;
                    }

                    #endregion Mouse Movement
                }

                if(settings.shortcuts.IsGuide()) {

                    settings.compositionGuideIndex++;

                    if (settings.compositionGuideIndex >= settings.compositionGuideCollection.templates.Count)
                    {
                        settings.compositionGuideIndex = 0;
                    }

                    UpdateCompositionGuideOverlay();

                }

                if (settings.shortcuts.IsAutoFocus())
                {

                    // iterate through the AutoFocusMode enum items
                    settings.autoFocus.mode++;

                    int enumSize = System.Enum.GetNames(typeof(AutoFocusSettings.Mode)).Length;
                    if ((int)settings.autoFocus.mode >= enumSize)
                    {
                        settings.autoFocus.mode = 0;
                    }

                    UpdateAutoFocusSetup();

                }

                // manual key: first key press => switch to manual mode
                if (settings.shortcuts.IsManualFocus())
                {
                    settings.autoFocus.mode = AutoFocusSettings.Mode.ManualPosition;
                    UpdateAutoFocusSetup();
                }

                // manual key: update focus position while the user presses the key
                if (settings.shortcuts.IsManualFocus())
                {
                    manualFocusPosition = settings.shortcuts.GetMousePosition();
                }

                #region Screenshot

                // left mouse button takes screenshots
                if (settings.shortcuts.IsMouseLeftDown())
                {
                    StartCoroutine( CaptureScreenshot());
                }

                #endregion Screenshot
                
            }

            UpdateAutoFocusData();

            UpdateModules();

            UpdateTextComponents();
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
			screenshot.Capture(settings);

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

        void OnDrawGizmos()
        {
            OnDrawGizmosModules();
        }

        void StartModules()
        {
            modules.ForEach(x => x.Start(this));
        }

        void UpdateModules()
        {
            modules.ForEach(x => x.Update());
        }

        void OnDrawGizmosModules() {
            modules.ForEach(x => x.OnDrawGizmos());
        }

        void EnableModules()
        {
            modules.ForEach(x => x.OnEnable());
        }

        void DisableModules() {
            modules.ForEach(x => x.OnDisable());
        }

        private void UpdateTextComponents()
        {
            UpdateImageSettingsText();
            UpdateAutoFocusText();
        }

        private void UpdateImageSettingsText() {

            if (!settings.imageSettingsText)
                return;

            string resolutionText = settings.resolution.GetEnumAttribute<InspectorNameAttribute>(settings.resolution).displayName;
            string textString = string.Format("Type: {0}\nFormat: {1}\nResolution: {2}", settings.photoType, settings.outputFormat, resolutionText);

            settings.imageSettingsText.text = textString;

        }

        private void UpdateAutoFocusText()
        {

            if (!settings.autoFocusText)
                return;

            string modeText = settings.resolution.GetEnumAttribute<InspectorNameAttribute>(settings.autoFocus.mode).displayName;
            string maxRayLengthText = autoFocusData.maxRayLength.ToString( "F2", numberFormatInfo);
            string minDistanceText = autoFocusData.IsTargetInRange() ? autoFocusData.minDistance.ToString("F2", numberFormatInfo) : "-";

            string textString = string.Format("Auto Focus: {0}\nMax Ray Length: {1}\nMin Distance: {2}", modeText, maxRayLengthText, minDistanceText);

            settings.autoFocusText.text = textString;

        }

        void UpdateCompositionGuideOverlay()
        {
            if (!settings.compositionGuideImage)
                return;

            if (!settings.compositionGuideCollection)
                return;

            if (settings.compositionGuideIndex >= settings.compositionGuideCollection.templates.Count) {
				Debug.LogError(string.Format( "Composition guide index out of range: {0} >= {1}", settings.compositionGuideIndex, settings.compositionGuideCollection.templates));
                return;
			}

            if (settings.compositionGuideIndex < 0)
            {
				Debug.Log("TODO: disable overlay");
                return;
            }

            CompositionGuideTemplate template = settings.compositionGuideCollection.templates[settings.compositionGuideIndex];

            settings.compositionGuideImage.sprite = template.sprite;
            settings.compositionGuideImage.color = template.color;

        }

        void UpdateAutoFocusSetup()
        {
            // update internal data structure; used for overlay and autofocus calculation (even without overlay)
            autoFocusInput.focusRays = settings.autoFocus.mode.GetRays();
            autoFocusInput.maxRayLength = settings.autoFocus.maxRayLength;
            autoFocusInput.layerMask = settings.autoFocus.layerMask;
        }

        void UpdateAutoFocusData() 
        {
            if(settings.autoFocus.mode == AutoFocusSettings.Mode.ManualPosition)
            {
                // use screen position of mouse
                AutoFocusCalculation.UpdateOutputData(settings.photoCamera, this.autoFocusInput, manualFocusPosition, ref this.autoFocusData);
            }
            else
            {
                // recalculate auto focus data. the data are used eg in the AF overlay and the DoF effect
                AutoFocusCalculation.UpdateOutputData(settings.photoCamera, this.autoFocusInput, ref this.autoFocusData);
            }
        }

        /// <summary>
        /// Container for the transform data of the player and the photo camera.
        /// States are saved and restored back and forth when the photo session functionality gets toggled on and off
        /// 
        /// The colliders on the camera will be disabled. Otherwise when we exit photo session mode the camera keeps on moving forward when a collider is active.
        /// </summary>
        private class CameraState
        {

            private Transform parent = null;
            private Vector3 position;
            private Quaternion rotation;
            private List<Collider> enabledColliders = new List<Collider>();

            public void Save(Transform transform)
            {
                parent = transform.parent;
                position = transform.position;
                rotation = transform.rotation;

                // register & disable all enabled colliders
                enabledColliders.Clear();

                Collider[] colliders = transform.gameObject.GetComponents<Collider>();
                foreach ( Collider collider in colliders)
                {
                    if( collider.enabled)
                    {
                        enabledColliders.Add(collider);

                        if( !collider.isTrigger)
                        {
                            Debug.LogWarning("Collider on camera isn't trigger. This may lead to unexpected camera movement after photo session exits");
                        }

                    }
                }

                enabledColliders.ForEach(x => x.enabled = false);
            }

            public void Restore(Transform transform)
            {
                transform.parent = parent;
                transform.position = position;
                transform.rotation = rotation;

                // disable and unregister all registered colliders
                enabledColliders.ForEach(x => x.enabled = true);
                enabledColliders.Clear();
            }

        }

        /// <summary>
        /// Container for the cursor visibility and lock state
        /// </summary>
        private class CursorState
        {

            private bool visible;
            private CursorLockMode lockMode;

            /// <summary>
            /// Save initial state, i. e. before photo session gets enabled
            /// </summary>
            public void Save()
            {
                visible = Cursor.visible;
                lockMode = Cursor.lockState;
            }

            /// <summary>
            /// Restore initial state, i. e. after photo session gets disabled
            /// </summary>
            public void Restore()
            {
                Cursor.visible = visible;
                Cursor.lockState = lockMode;
            }

            /// <summary>
            /// Lock cursor mode eg for camera movement
            /// </summary>
            public void Lock()
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            /// <summary>
            /// Unlock cursor mode for menu usage
            /// </summary>
            public void Unlock()
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

        }
    }
}