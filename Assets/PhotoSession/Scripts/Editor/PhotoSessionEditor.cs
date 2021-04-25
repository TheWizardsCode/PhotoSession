using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Rowlan.PhotoSession.ImageResolution;

namespace Rowlan.PhotoSession
{
    [CustomEditor(typeof(PhotoSession))]
    public class PhotoSessionEditor : BaseEditor<PhotoSession>
    {
        private static readonly string tag_MainCamera = "MainCamera";

        private static readonly string[] script_Blacklist = {
            "SimpleCameraController", // Unity HDRP Demo
            "LookWithMouse", // Unity HDRP Demo
            "CharacterController", // Unity HDRP Demo
            "PlayerMovement", // Unity HDRP Demo
            "MFreeLookCamera", // Malbers FreeLookCameraRig
            "CameraWallStop", // Malbers FreeLookCameraRig
            };

        Hdrp.DepthOfFieldModuleEditor hdrpDepthOfFieldModuleEditor = new Hdrp.DepthOfFieldModuleEditor();
        Urp.DepthOfFieldModuleEditor urpDepthOfFieldModuleEditor = new Urp.DepthOfFieldModuleEditor();
        Legacy.DepthOfFieldModuleEditor legacyDepthOfFieldModuleEditor = new Legacy.DepthOfFieldModuleEditor();

        PhotoSession editorTarget;
        PhotoSessionEditor editor;

        SerializedProperty resolution;
        SerializedProperty aspectRatio;
        SerializedProperty photoType;
        SerializedProperty outputFormat;

        SerializedProperty photoCamera;
        SerializedProperty fieldOfViewOverride;
        SerializedProperty fieldOfView;

        SerializedProperty toggleKey;

        SerializedProperty movementSpeed;
        SerializedProperty movementSpeedFast;
        SerializedProperty freeLookSensitivity;
        SerializedProperty zoomSensitivity;
        SerializedProperty zoomSensitivityFast;

        SerializedProperty pauseTime;
        SerializedProperty reusePreviousCameraTransform;
        SerializedProperty disabledComponents;
        SerializedProperty canvas;
        SerializedProperty imageSettingsText;
        SerializedProperty compositionGuideImage;
        SerializedProperty compositionGuideCollection;
        SerializedProperty compositionGuideIndex;

        SerializedProperty hdrpDepthOfFieldSettings;
        SerializedProperty urpDepthOfFieldSettings;
        SerializedProperty legacyDepthOfFieldSettings;

        bool disabledComponentsExpanded = true;

        public void OnEnable()
        {
            editor = this;
            editorTarget = (PhotoSession)target;

            // properties
            resolution = FindProperty(x => x.settings.resolution);
            aspectRatio = FindProperty(x => x.settings.aspectRatio);
            photoType = FindProperty(x => x.settings.photoType);
            outputFormat = FindProperty(x => x.settings.outputFormat);
            fieldOfViewOverride = FindProperty(x => x.settings.fieldOfViewOverride);
            fieldOfView = FindProperty(x => x.settings.fieldOfView);
            toggleKey = FindProperty(x => x.settings.toggleKey);
            photoCamera = FindProperty(x => x.settings.photoCamera);
            movementSpeed = FindProperty(x => x.settings.movementSpeed);
            movementSpeedFast = FindProperty(x => x.settings.movementSpeedFast);
            freeLookSensitivity = FindProperty(x => x.settings.freeLookSensitivity);
            zoomSensitivity = FindProperty(x => x.settings.zoomSensitivity);
            zoomSensitivityFast = FindProperty(x => x.settings.zoomSensitivityFast);
            pauseTime = FindProperty(x => x.settings.pauseTime);
            reusePreviousCameraTransform = FindProperty(x => x.settings.reusePreviousCameraTransform);
            disabledComponents = FindProperty(x => x.settings.disabledComponents);
            canvas = FindProperty(x => x.settings.canvas);
            imageSettingsText = FindProperty(x => x.settings.imageSettingsText);

            compositionGuideImage = FindProperty(x => x.settings.compositionGuideImage);
            compositionGuideCollection = FindProperty(x => x.settings.compositionGuideCollection);
            compositionGuideIndex = FindProperty(x => x.settings.compositionGuideIndex);

            // modules            

            #region HDRP
            hdrpDepthOfFieldModuleEditor.OnEnable(editor, editorTarget);
            hdrpDepthOfFieldSettings = FindProperty(x => x.settings.hdrpDepthOfFieldSettings);
            #endregion HDRP

            #region URP
            urpDepthOfFieldModuleEditor.OnEnable(editor, editorTarget);
            urpDepthOfFieldSettings = FindProperty(x => x.settings.urpDepthOfFieldSettings);
            #endregion URP

            #region Legacy
            legacyDepthOfFieldModuleEditor.OnEnable(editor, editorTarget);
            legacyDepthOfFieldSettings = FindProperty(x => x.settings.legacyDepthOfFieldSettings);
            #endregion Legacy

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // base.DrawDefaultInspector();

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Image", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(resolution, new GUIContent("Resolution"));

                if (resolution.intValue != (int) ImageResolutionType.Game)
                {
                    EditorGUILayout.PropertyField(aspectRatio, new GUIContent("Aspect Ratio"));
                }

                EditorGUILayout.PropertyField(outputFormat, new GUIContent("Output Format"));
                
                EditorGUILayout.PropertyField(photoType, new GUIContent("Photo Type"));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Camera", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(photoCamera, new GUIContent("Photo Camera", "The camera that will be used for the photo session. Usually the main camera"));

                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(fieldOfViewOverride, new GUIContent("Field of View"));
                    if (fieldOfViewOverride.boolValue)
                    {
                        EditorGUILayout.PropertyField(fieldOfView, GUIContent.none);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Input", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(toggleKey, new GUIContent("Toggle Key", "Input key which toggles the photo mode"));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Navigation", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(movementSpeed, new GUIContent("Movement Speed", "Normal camera movement speed"));


                EditorGUILayout.PropertyField(movementSpeedFast, new GUIContent("Movement Speed Fast", "Camera movement speed when shift is pressed"));

                EditorGUILayout.PropertyField(freeLookSensitivity, new GUIContent("Free Look Sensitivity", "Mouse look sensitivity"));


                EditorGUILayout.PropertyField(zoomSensitivity, new GUIContent("Zoom Sensitivity", "Normal mouse wheel zoom amount"));

                EditorGUILayout.PropertyField(zoomSensitivityFast, new GUIContent("Zoom Sensitivity Fast", "Mouse wheel zoom amount when shift is pressed"));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Pause", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(pauseTime, new GUIContent("Time Scale", "The scaled time factor in Photo Mode. Use 0 for full pause"));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Initialization", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(reusePreviousCameraTransform, new GUIContent("Reuse Prev. Camera Transform", "Keep the photo camera position and rotation the way it was in the previous session"));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("GameObject Modification", GUIStyles.BoxTitleStyle);

                // TODO: find out how to use Unity's internal mechanism for arrays
                // EditorGUILayout.PropertyField(disabledComponents, new GUIContent("Disabled Components", "Objects(e.g.Scripts) which should be disabled when photo mode is activated"));

                disabledComponentsExpanded = EditorGUILayout.Foldout(disabledComponentsExpanded, new GUIContent( "Disabled Components"), true);
                if (disabledComponentsExpanded)
                {
                    disabledComponents.arraySize = EditorGUILayout.IntField("Size", disabledComponents.arraySize);
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < disabledComponents.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(disabledComponents.GetArrayElementAtIndex(i), new GUIContent(string.Format("Element {0}", i)));
                    }
                    EditorGUI.indentLevel--;
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("User Interface", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(canvas, new GUIContent("Canvas", "An optional canvas with a white stretched image. The alpha value of the image will be used to simulate a flash effect. The canvas can be hidden"));
                EditorGUILayout.PropertyField(imageSettingsText, new GUIContent("Image Settings Text", "An optional Text object which shows the screenshot image information"));

                EditorGUILayout.PropertyField(compositionGuideImage, new GUIContent("Composition Guide Image", "The image which will be used for the composition guide overlay"));
                EditorGUILayout.PropertyField(compositionGuideCollection, new GUIContent("Composition Guide Collection", "A collection of composition guides which can be used as overlay"));
                EditorGUILayout.PropertyField(compositionGuideIndex, new GUIContent("Composition Guide Index", "The currently used composition guide of the collection"));
                
            }
            GUILayout.EndVertical();

            #region Modules

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Effects", GUIStyles.BoxTitleStyle);

                OnInspectorGUIModules();
            }
            GUILayout.EndVertical();

            #endregion Modules

            EditorGUILayout.BeginVertical( "box");
            {
                EditorGUILayout.LabelField("Tools", GUIStyles.BoxTitleStyle);

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Auto Setup"))
                    {
                        PerformAutoSetup();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnInspectorGUIModules() {

#if USING_HDRP

            hdrpDepthOfFieldModuleEditor.OnInspectorGUI();

#endif

#if USING_URP

            urpDepthOfFieldModuleEditor.OnInspectorGUI();

#endif

#if USING_LEGACY

            legacyDepthOfFieldModuleEditor.OnInspectorGUI();

#endif

        }

        private void PerformAutoSetup()
        {
            AssignMainCamera();

            AssignScriptBlacklist();
        }

        private void AssignMainCamera() {

            // find all cameras
            Camera[] cameras = Object.FindObjectsOfType<Camera>();

            Camera mainCamera = null;
            foreach (Camera camera in cameras)
            {

                // skip all inaktive
                if (!camera.isActiveAndEnabled)
                    continue;

                // last one wins unless it's tagged as maincamera, then that one wins
                if (!mainCamera || camera.CompareTag(tag_MainCamera))
                {
                    mainCamera = camera;
                }
                
            }
            
            if (mainCamera)
            {
                this.photoCamera.objectReferenceValue = mainCamera;
            }
        }

        private void AssignScriptBlacklist() {

            Camera photoCamera = this.photoCamera.objectReferenceValue as Camera;

            if (!photoCamera)
                return;

            List<Component> components = new List<Component>();

            foreach (string scriptName in script_Blacklist) {

                // recursively search the blacklisted objects from the photo camera gameobject up to the root
                Transform transform = photoCamera.transform;
                while(transform) {

                    Component component = transform.gameObject.GetComponent(scriptName);

                    if( component)
                        components.Add(component);

                    transform = transform.parent;
                }
            }

            // set the disabled components in the serializedObject
            this.disabledComponents.ClearArray();

            for ( int i=0; i < components.Count; i++) {

                this.disabledComponents.InsertArrayElementAtIndex(i);
                this.disabledComponents.GetArrayElementAtIndex(i).objectReferenceValue = components[i];

            }

        }
    }
}