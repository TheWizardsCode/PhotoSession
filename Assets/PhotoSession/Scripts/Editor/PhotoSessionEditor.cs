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

        SerializedProperty movementSpeed;
        SerializedProperty movementSpeedFast;
        SerializedProperty freeLookSensitivity;
        SerializedProperty zoomSensitivity;
        SerializedProperty zoomSensitivityFast;

        SerializedProperty pauseTime;
        SerializedProperty reusePreviousCameraTransform;
        SerializedProperty disabledComponents;
        SerializedProperty canvas;
        SerializedProperty flashImage;
        SerializedProperty displayImage;
        SerializedProperty imageSettingsText;
        SerializedProperty autoFocusText;
        SerializedProperty compositionGuideImage;
        SerializedProperty compositionGuideCollection;
        SerializedProperty compositionGuideIndex;

        SerializedProperty autoFocusMode;
        SerializedProperty autoFocusMaxRayLength;
        SerializedProperty autoFocusOverlayVisible;
        SerializedProperty autoFocusOverlayMaterial;
        SerializedProperty autoFocusLayerMask;

        SerializedProperty toggleKeyCode;
        SerializedProperty guideKeyCode;
        SerializedProperty autoFocusKeyCode;
        SerializedProperty manualFocusKeyCode;

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
            flashImage = FindProperty(x => x.settings.flashImage);
            displayImage = FindProperty(x => x.settings.displayImage);
            imageSettingsText = FindProperty(x => x.settings.imageSettingsText);
            autoFocusText = FindProperty(x => x.settings.autoFocusText);

            compositionGuideImage = FindProperty(x => x.settings.compositionGuideImage);
            compositionGuideCollection = FindProperty(x => x.settings.compositionGuideCollection);
            compositionGuideIndex = FindProperty(x => x.settings.compositionGuideIndex);

            autoFocusMode = FindProperty(x => x.settings.autoFocus.mode);
            autoFocusMaxRayLength = FindProperty(x => x.settings.autoFocus.maxRayLength);
            autoFocusOverlayVisible = FindProperty(x => x.settings.autoFocus.overlayVisible);
            autoFocusOverlayMaterial = FindProperty(x => x.settings.autoFocus.overlayMaterial);
            autoFocusLayerMask = FindProperty(x => x.settings.autoFocus.layerMask);

            toggleKeyCode = FindProperty(x => x.settings.shortcuts.toggleKeyCode);
            guideKeyCode = FindProperty(x => x.settings.shortcuts.guideKeyCode);
            autoFocusKeyCode = FindProperty(x => x.settings.shortcuts.autoFocusKeyCode);
            manualFocusKeyCode = FindProperty(x => x.settings.shortcuts.manualFocusKeyCode);

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

                EditorGUILayout.LabelField("Auto Focus");

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(autoFocusMode, new GUIContent("Mode", "The auto focus mode used e. g. for the Depth of Field effect"));
                EditorGUILayout.PropertyField(autoFocusMaxRayLength, new GUIContent("Max Ray Length", "The maximum raycast length for focus calculation"));
                EditorGUILayout.PropertyField(autoFocusLayerMask, new GUIContent("Layer Mask", "The layer mask for the raycast"));
                
                EditorGUILayout.HelpBox("Note: Auto Focus requires Depth of Field Effect", MessageType.None);

                EditorGUI.indentLevel--;

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


            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Shortcuts", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(toggleKeyCode, new GUIContent("Toggle Key", "Input key which toggles the photo mode"));

                EditorGUILayout.PropertyField(guideKeyCode, new GUIContent("Guide", "Toggle through available guides"));
                EditorGUILayout.PropertyField(autoFocusKeyCode, new GUIContent("Auto Focus", "Toggle through auto focus grid options"));
                EditorGUILayout.PropertyField(manualFocusKeyCode, new GUIContent("Manual Focus", "Keep key pressed and move mouse to set focus manually"));

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
                EditorGUILayout.LabelField("User Interface", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(canvas, new GUIContent("Canvas", "An optional canvas with a white stretched image. The alpha value of the image will be used to simulate a flash effect. The canvas can be hidden"));

                EditorGUILayout.PropertyField(flashImage, new GUIContent("Flash Image", "The flash image which is used to simulate a flash when a photo is taken"));

                EditorGUILayout.LabelField("Display");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(displayImage, new GUIContent("Image", "The display of the photo session camera. Contains e. g. focus rectangles and settings text"));
                EditorGUILayout.PropertyField(imageSettingsText, new GUIContent("Image Settings Text", "An optional Text object which shows the screenshot image information"));
                EditorGUILayout.PropertyField(autoFocusText, new GUIContent("Auto Focus Text", "An optional Text object which shows auto focus information"));
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Composition Guide");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(compositionGuideImage, new GUIContent("Image", "The image which will be used for the composition guide overlay"));
                EditorGUILayout.PropertyField(compositionGuideCollection, new GUIContent("Collection", "A collection of composition guides which can be used as overlay"));
                EditorGUILayout.PropertyField(compositionGuideIndex, new GUIContent("Index", "The currently used composition guide of the collection"));
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Auto Focus");
                EditorGUI.indentLevel++;
                // the mode itself is part of the user interaction, not the setup
                // EditorGUILayout.PropertyField(autoFocusMode, new GUIContent("Mode", "The auto focus mode used among others for the overlay image"));
                EditorGUILayout.PropertyField(autoFocusOverlayVisible, new GUIContent("Overlay Visible", "Whether the auto focus overlay is visible or not"));
                EditorGUILayout.PropertyField(autoFocusOverlayMaterial, new GUIContent("Overlay Material", "The material which will be used for the auto focus overlay display"));
                EditorGUI.indentLevel--;

            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("GameObject Modification", GUIStyles.BoxTitleStyle);

                // TODO: find out how to use Unity's internal mechanism for arrays
                // EditorGUILayout.PropertyField(disabledComponents, new GUIContent("Disabled Components", "Objects(e.g.Scripts) which should be disabled when photo mode is activated"));

                disabledComponentsExpanded = EditorGUILayout.Foldout(disabledComponentsExpanded, new GUIContent("Disabled Components"), true);
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

            EditorGUILayout.BeginVertical( "box");
            {
                EditorGUILayout.LabelField("Tools", GUIStyles.BoxTitleStyle);

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Auto Setup"))
                    {
                        AutoSetup.Execute( this.editorTarget);
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

    }
}