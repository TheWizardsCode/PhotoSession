using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.PhotoSession
{
    /// <summary>
    /// Common utils for layer handling
    /// </summary>
    public class LayerUtils
    {
        /// <summary> 
        /// Specific layer values
        /// </summary>
        public enum LayerIndex
        {
            Nothing = 0,
            Everything = int.MaxValue,
            IgnoreRaycast = 2,
        }

        /// <summary>
        /// Set the layer on parent and recursively on all children
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="layer"></param>
        public static void SetLayer( Transform parent, int layer)
        {
            parent.gameObject.layer = layer;

            for (int i = 0; i < parent.childCount; i++)
            {
                SetLayer(parent.GetChild(i), layer);
            }
        }

    }
}
