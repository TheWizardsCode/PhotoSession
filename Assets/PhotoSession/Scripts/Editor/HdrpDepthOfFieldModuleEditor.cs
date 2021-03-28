using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rowlan.PhotoSession
{
    public class HdrpDepthOfFieldModuleEditor
    {
        private PhotoSession editorTarget;
        private PhotoSessionEditor editor;

        private SerializedProperty featureEnabled;
        private SerializedProperty volume;
        private SerializedProperty maxFocusDistance;
        private SerializedProperty hitDistanceNearFocusEndOffset;
        private SerializedProperty hitDistanceFarFocusStartOffset;
        private SerializedProperty hitDistanceFarFocusEndOffset;

        public void OnEnable(PhotoSessionEditor editor, PhotoSession editorTarget) {
#if USING_HDRP            
            this.editor = editor;
            this.editorTarget = editorTarget;

            featureEnabled = editor.FindProperty(x => x.settings.hdrpDepthOfFieldSettings.featureEnabled);
            volume = editor.FindProperty(x => x.settings.hdrpDepthOfFieldSettings.volume);
            maxFocusDistance = editor.FindProperty(x => x.settings.hdrpDepthOfFieldSettings.maxFocusDistance);
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
                EditorGUILayout.LabelField("HDRP Depth of Field", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(featureEnabled, new GUIContent("Enabled", ""));

                if (featureEnabled.boolValue) 
                { 

                    EditorGUILayout.PropertyField(volume, new GUIContent("Volume", "The volume with the Depth of Field settings"));

                    EditorGUILayout.LabelField("Hit Target");
                    EditorGUI.indentLevel++;
                    {
                        EditorGUILayout.PropertyField(maxFocusDistance, new GUIContent("Max Focus Distance", ""));
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
    }
}
