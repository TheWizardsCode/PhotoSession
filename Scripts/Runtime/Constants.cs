using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.PhotoSession
{
    /// <summary>
    /// Collection of constants.
    /// e. g. paths which we have to consider if we refactor the package structure
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Location of the Photo Session prefab which can be added to the scene via the context menu for quick setup
        /// </summary>
        public static string PhotoSessionPrefabPath = "Prefabs/Photo Session";

		#region Composition Guides

		public const string CompositionGuideTemplateCollection_FileName = "Composition Guide Collection";
        public const string CompositionGuideTemplateCollection_MenuName = "Photo Session/Templates/Composition Guide Collection";

        public const string CompositionGuideTemplate_FileName = "Composition Guide Template";
        public const string CompositionGuideTemplate_MenuName = "Photo Session/Templates/Composition Guide Template";

        #endregion Composition Guides
    }
}