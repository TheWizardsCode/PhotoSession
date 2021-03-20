using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static Rowlan.PhotoSession.ImageResolution;

namespace Rowlan.PhotoSession
{
    [CustomEditor(typeof(PhotoSession))]
    public class PhotoSessionEditor : BaseEditor<PhotoSession>
    {
        private static readonly string tag_MainCamera = "MainCamera";

        private static readonly string[] script_Blacklist = {
            "MFreeLookCamera", // Malbers FreeLookCameraRig
            "CameraWallStop", // Malbers FreeLookCameraRig
            };

        PhotoSession editorTarget;
        PhotoSessionEditor editor;

        SerializedProperty resolution;
        SerializedProperty aspectRatio;
        SerializedProperty outputFormat;
        SerializedProperty toggleKey;
        SerializedProperty photoCamera;
        SerializedProperty movementSpeed;
        SerializedProperty movementSpeedFast;
        SerializedProperty freeLookSensitivity;
        SerializedProperty zoomSensitivity;
        SerializedProperty zoomSensitivityFast;
        SerializedProperty pauseTime;
        SerializedProperty reusePreviousCameraTransform;
        SerializedProperty disabledComponents;
        SerializedProperty canvas;

        public void OnEnable()
        {
            editor = this;
            editorTarget = (PhotoSession)target;

            resolution = FindProperty(x => x.settings.resolution);
            aspectRatio = FindProperty(x => x.settings.aspectRatio);
            outputFormat = FindProperty(x => x.settings.outputFormat);
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
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Image", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(resolution, new GUIContent("Resolution"));

                if (resolution.intValue != (int) ImageResolutionType.Game)
                {
                    EditorGUILayout.PropertyField(aspectRatio, new GUIContent("Aspect Ratio"));
                }

                EditorGUILayout.PropertyField(outputFormat, new GUIContent("Output Format"));
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
                EditorGUILayout.LabelField("Camera Settings", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(photoCamera, new GUIContent("Photo Camera", "The camera that will be used for the photo session. Usually the main camera"));

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
                EditorGUILayout.PropertyField(disabledComponents, new GUIContent("Disabled Components", "Objects(e.g.Scripts) which should be disabled when photo mode is activated"));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("User Interface", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(canvas, new GUIContent("Canvas", "An optional canvas with a white stretched image. The alpha value of the image will be used to simulate a flash effect. The canvas can be hidden"));
            }
            GUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();
            {
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

            for ( int i=0; i <components.Count; i++) {

                this.disabledComponents.InsertArrayElementAtIndex(i);
                this.disabledComponents.GetArrayElementAtIndex(i).objectReferenceValue = components[i];

            }

        }
    }
}