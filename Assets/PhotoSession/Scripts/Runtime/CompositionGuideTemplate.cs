using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rowlan.PhotoSession
{
	[CreateAssetMenu(fileName = Constants.CompositionGuideTemplate_FileName, menuName = Constants.CompositionGuideTemplate_MenuName)]
	[System.Serializable]
	public class CompositionGuideTemplate : ScriptableObject
	{
		/// <summary>
		/// The name used in the UI
		/// </summary>
		public string templateName;

		/// <summary>
		/// The image which is used as overlay
		/// </summary>
		public Sprite sprite;

		/// <summary>
		/// The color for the overlay
		/// </summary>
		public Color color;
	}
}