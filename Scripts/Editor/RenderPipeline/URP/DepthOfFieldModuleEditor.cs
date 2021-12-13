using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rowlan.PhotoSession.Urp
{
    public class DepthOfFieldModuleEditor
    {
        private PhotoSession editorTarget;
        private PhotoSessionEditor editor;

        private SerializedProperty featureEnabled;
        private SerializedProperty volume;
        private SerializedProperty focusDistanceOffset;
        private SerializedProperty focalLength;
        private SerializedProperty aperture;

        public void OnEnable(PhotoSessionEditor editor, PhotoSession editorTarget) {
#if USING_URP            
            this.editor = editor;
            this.editorTarget = editorTarget;

            featureEnabled = editor.FindProperty(x => x.settings.urpDepthOfFieldSettings.featureEnabled);
            volume = editor.FindProperty(x => x.settings.urpDepthOfFieldSettings.volume);
            focusDistanceOffset = editor.FindProperty(x => x.settings.urpDepthOfFieldSettings.focusDistanceOffset);
            focalLength = editor.FindProperty(x => x.settings.urpDepthOfFieldSettings.focalLength);
            aperture = editor.FindProperty(x => x.settings.urpDepthOfFieldSettings.aperture);
#endif
        }

        public void OnInspectorGUI() 
        {
#if USING_URP
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

                    EditorGUILayout.PropertyField(volume, new GUIContent("Volume", "The volume with the Depth of Field settings"));

                    EditorGUILayout.LabelField("Hit Target");
                    EditorGUI.indentLevel++;
                    {
                        EditorGUILayout.PropertyField(focusDistanceOffset, new GUIContent("Focus Distance Offset", ""));
                        EditorGUILayout.PropertyField(focalLength, new GUIContent("Focal Length", "The focal length override"));
                        EditorGUILayout.PropertyField(aperture, new GUIContent("Aperture", "The aperture override"));
                    }
                    EditorGUI.indentLevel--;
                }
            }
            GUILayout.EndVertical();
#endif
        }
    }
}
