using UnityEditor;
using UnityEngine;

namespace Rowlan.PhotoSession
{
    public class GUIStyles
    {

        private static GUIStyle _boxTitleStyle;
        public static GUIStyle BoxTitleStyle
        {
            get
            {
                if (_boxTitleStyle == null)
                {
                    _boxTitleStyle = new GUIStyle("Label");
                    _boxTitleStyle.fontStyle = FontStyle.BoldAndItalic;
                }
                return _boxTitleStyle;
            }
        }

        private static GUIStyle _groupTitleStyle;
        public static GUIStyle GroupTitleStyle
        {
            get
            {
                if (_groupTitleStyle == null)
                {
                    _groupTitleStyle = new GUIStyle("Label");
                    _groupTitleStyle.fontStyle = FontStyle.Bold;
                }
                return _groupTitleStyle;
            }
        }

        public static Color DefaultBackgroundColor = GUI.backgroundColor;
        public static Color ErrorBackgroundColor = new Color( 1f,0f,0f,0.7f); // red tone

		#region LabelFontStyle
		private static FontStyle prevLabelFontStyle;

        public static void BeginLabelFontStyle(FontStyle fontStyle) 
        {
            prevLabelFontStyle = EditorStyles.label.fontStyle;
            EditorStyles.label.fontStyle = fontStyle;
        }

        public static void EndLabelFontStyle()
        {
            EditorStyles.label.fontStyle = prevLabelFontStyle;
        }
        #endregion LabelFontStyle

    }
}