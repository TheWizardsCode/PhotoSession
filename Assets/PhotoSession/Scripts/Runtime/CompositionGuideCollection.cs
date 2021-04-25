using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rowlan.PhotoSession
{
	[CreateAssetMenu(fileName = Constants.CompositionGuideTemplateCollection_FileName, menuName = Constants.CompositionGuideTemplateCollection_MenuName)]
	[System.Serializable]
	public class CompositionGuideCollection : ScriptableObject
	{
		/// <summary>
		/// Collection of various composition guide templates
		/// </summary>
		[SerializeField]
		public List<CompositionGuideTemplate> templates = new List<CompositionGuideTemplate>();
	}
}