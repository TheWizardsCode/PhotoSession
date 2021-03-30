using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Rowlan.PhotoSession.ImageResolution;

namespace Rowlan.PhotoSession
{
	[System.Serializable]
	public class ImageResolution
	{
		public static ImageResolution PresetGame = new ImageResolution(ImageResolutionType.Game, -1, -1);
		public static ImageResolution Preset1080p = new ImageResolution(ImageResolutionType.Resolution_1080p, 120 * 16, 120 * 9);
		public static ImageResolution Preset4K = new ImageResolution(ImageResolutionType.Resolution_4K, 120 * 16 * 2, 120 * 9 * 2);
		public static ImageResolution Preset5K = new ImageResolution(ImageResolutionType.Resolution_5K, 320 * 16, 320 * 9);
		public static ImageResolution Preset8K = new ImageResolution(ImageResolutionType.Resolution_8K, 120 * 16 * 4, 120 * 9 * 4);
		public static ImageResolution Preset10K = new ImageResolution(ImageResolutionType.Resolution_10K, 120 * 16 * 5, 120 * 9 * 5);
		public static ImageResolution Preset12K = new ImageResolution(ImageResolutionType.Resolution_12K, 120 * 16 * 6, 120 * 9 * 6);
		public static ImageResolution Preset16K = new ImageResolution(ImageResolutionType.Resolution_16K, 120 * 16 * 8, 120 * 9 * 8);

		public static ImageResolution[] imageResolutions = new ImageResolution[] {
			PresetGame, Preset1080p, Preset4K, Preset5K, Preset8K, Preset10K, Preset12K, Preset16K
		};

		public enum AspectRatio {
			
			[InspectorName("16:9")]
			AR_16_9,

			[InspectorName("16:10")]
			AR_16_10,

			[InspectorName("21:9")]
			AR_21_9,

			[InspectorName("32:9")]
			AR_32_9,

			[InspectorName("4:3")]
			AR_4_3,

			[InspectorName("1:1")]
			AR_1_1,

		}

		public enum ImageResolutionType
		{
			[InspectorName("Game Screen")]
			Game,

			[InspectorName("1080p (1920 x 1080)")]
			Resolution_1080p,

			[InspectorName("4K (3840 x 2160)")]
			Resolution_4K,

			[InspectorName("5K (5120 x 2880)")]
			Resolution_5K,

			[InspectorName("8K (7680 x 4320)")]
			Resolution_8K,

			[InspectorName("10K (9600 x 5400)")]
			Resolution_10K,

			[InspectorName("12K (11520 x 6480)")]
			Resolution_12K,

			[InspectorName("16K (15360 x 8640)")]
			Resolution_16K,

		}

		public ImageResolutionType Type { get; }
		public int Width { get; }
		public int Height { get; }

		public ImageResolution(ImageResolutionType type, int width, int height)
		{
			Type = type;
			Width = width;
			Height = height;
		}

	}

	public static class ImageResolutionExtension
	{
		public static float GetRatio(this AspectRatio aspectRatio)
		{
			switch (aspectRatio)
			{
				case AspectRatio.AR_16_9: return 16f / 9f;
				case AspectRatio.AR_16_10: return 16f / 10f;
				case AspectRatio.AR_21_9: return 21f / 9f;
				case AspectRatio.AR_32_9: return 32f / 9f;
				case AspectRatio.AR_4_3: return 4f / 3f;
				case AspectRatio.AR_1_1: return 1f;
				default: throw new ArgumentException("Aspect ratio not defined: " + aspectRatio);
			}
		}

		public static ImageResolution GetImageResolution(this ImageResolution.ImageResolutionType imageResolutionType, AspectRatio aspectRatio)
		{
			ImageResolution baseImageResolution = ImageResolution.imageResolutions.ToList<ImageResolution>().Find(x => x.Type == imageResolutionType);

			int height = baseImageResolution.Height;
			int width = (int) (aspectRatio.GetRatio() * height);

			ImageResolution aspectRatioImageResolution = new ImageResolution(imageResolutionType, width, height);

			if (aspectRatioImageResolution != null)
				return aspectRatioImageResolution;

			Debug.LogError("Resolution not found: " + baseImageResolution);

			return ImageResolution.PresetGame;
		}

	}
}
