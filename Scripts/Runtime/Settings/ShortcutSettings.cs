using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.PhotoSession
{
    [Serializable]
    public class ShortcutSettings
    {
        /// <summary>
        /// Input key which toggles the photo mode
        /// </summary>
        public KeyCode toggleKeyCode = KeyCode.F12;

        /// <summary>
        /// Show Guides
        /// </summary>
        public KeyCode guideKeyCode = KeyCode.G;

        /// <summary>
        /// Iterate through auto focus grids
        /// </summary>
        public KeyCode autoFocusKeyCode = KeyCode.F;

        /// <summary>
        /// Set manual focus by keeping key pressed and moving mouse
        /// </summary>
        public KeyCode manualFocusKeyCode = KeyCode.M;


        #region Input Accessors

        public bool IsTogglePhotoSession() => Input.GetKeyDown(toggleKeyCode);
        public bool IsGuide() => Input.GetKeyDown( guideKeyCode);

        public bool IsAutoFocus() => Input.GetKeyDown(autoFocusKeyCode);
        public bool IsManualFocus() => Input.GetKey(manualFocusKeyCode);

        public bool IsMoveFast() => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        public bool IsMoveLeft() => Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        public bool IsMoveRight() => Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        public bool IsMoveForward() => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        public bool IsMoveBack() => Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        public bool IsMoveDown() => Input.GetKey(KeyCode.Q);
        public bool IsMoveUp() => Input.GetKey(KeyCode.E);

        public bool IsMouseLeftDown() => Input.GetMouseButtonDown(0);
        public bool IsMouseRightDown() => Input.GetMouseButtonDown(1);
        public bool IsMouseRightUp() => Input.GetMouseButtonUp(1);
        public bool IsMouseRightPressed() => Input.GetMouseButton(1);

        public float GetMouseAxisX() => Input.GetAxis("Mouse X");
        public float GetMouseAxisY() => Input.GetAxis("Mouse Y");
        public float GetMouseAxisWheel() => Input.GetAxis("Mouse ScrollWheel");

        public Vector3 GetMousePosition() => Input.mousePosition;

        #endregion Input Accessors
    }
}
