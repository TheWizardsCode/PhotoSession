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
    }
}
