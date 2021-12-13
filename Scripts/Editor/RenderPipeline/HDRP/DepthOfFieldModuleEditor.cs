using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if USING_HDRP  
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor.Rendering;
#endif

namespace Rowlan.PhotoSession.Hdrp
{
    public class DepthOfFieldModuleEditor
    {
        private PhotoSession editorTarget;
        private PhotoSessionEditor editor;

        private SerializedProperty featureEnabled;
        private SerializedProperty volume;
        private SerializedProperty hitDistanceNearFocusEndOffset;
        private SerializedProperty hitDistanceFarFocusStartOffset;
        private SerializedProperty hitDistanceFarFocusEndOffset;

        public void OnEnable(PhotoSessionEditor editor, PhotoSession editorTarget) {
#if USING_HDRP            
            this.editor = editor;
            this.editorTarget = editorTarget;

            featureEnabled = editor.FindProperty(x => x.settings.hdrpDepthOfFieldSettings.featureEnabled);
            volume = editor.FindProperty(x => x.settings.hdrpDepthOfFieldSettings.volume);
            hitDistanceNearFocusEndOffset = editor.FindProperty(x => x.settings.hdrpDepthOfFieldSettings.hitDistanceNearFocusEndOffset);
            hitDistanceFarFocusStartOffset = editor.FindProperty(x => x.settings.hdrpDepthOfFieldSettings.hitDistanceFarFocusStartOffset);
            hitDistanceFarFocusEndOffset = editor.FindProperty(x => x.settings.hdrpDepthOfFieldSettings.hitDistanceFarFocusEndOffset);
#endif
        }

        public void OnInspectorGUI() 
        {
#if USING_HDRP
            GUILayout.BeginVertical("box");
            {
                GUIStyles.BeginLabelFontStyle(FontStyle.Bold);
                {
                    EditorGUILayout.PropertyField(featureEnabled, new GUIContent("Depth of Field", ""));
                }
                GUIStyles.EndLabelFontStyle();

                if (featureEnabled.boolValue) 
                { 
                    EditorGUILayout.HelpBox("The DoF effect is experimental and highly depends on your settings.", MessageType.Warning);

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(volume, new GUIContent("Volume", "The volume with the Depth of Field settings"));

                        if (GUILayout.Button("Create", EditorStyles.miniButton, GUILayout.Width(50)))
                        {
                            CreateDofVolume();
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("Hit Target");
                    EditorGUI.indentLevel++;
                    {
                        EditorGUILayout.PropertyField(hitDistanceNearFocusEndOffset, new GUIContent("Near Focus End Offset", ""));
                        EditorGUILayout.PropertyField(hitDistanceFarFocusStartOffset, new GUIContent("Far Focus Start Offset", ""));
                        EditorGUILayout.PropertyField(hitDistanceFarFocusEndOffset, new GUIContent("Far Focus End Offset", ""));
                    }
                    EditorGUI.indentLevel--;
                }
            }
            GUILayout.EndVertical();
#endif
        }

#if USING_HDRP
        private void CreateDofVolume()
        {
            // create the gameobject
            GameObject volumeGo = new GameObject("Photo Session Volume");

            // parent the gameobject to the photo session gameobject
            volumeGo.transform.parent = editorTarget.transform;

            // create a new Volume
            Volume effectVolume = volumeGo.AddComponent<Volume>();

            // global profile
            effectVolume.isGlobal = true;

            // create the volume profile
            effectVolume.profile = VolumeProfileFactory.CreateVolumeProfile(effectVolume.gameObject.scene, effectVolume.name);

            // create the effects
            DepthOfField effect = VolumeProfileFactory.CreateVolumeComponent<DepthOfField>(effectVolume.profile, true, false);

            // by default the effect is disabled, we don't want to modify the current game
            effect.active = false;

            // set the volume in the inspector
            volume.objectReferenceValue = effectVolume;

            // persist settings
            volume.serializedObject.ApplyModifiedProperties();

            // sharedprofile needs to be set, otherwise the profile would be lost in the created volume after we click play
            effectVolume.sharedProfile = effectVolume.profile;

            Debug.Log("Volume profile created: " + AssetDatabase.GetAssetPath(effectVolume.profile));

        }
#endif
    }
}
