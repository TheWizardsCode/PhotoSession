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
		public static ImageResolution PresetFullHd = new ImageResolution(ImageResolutionType.FullHd, 1920, 1080);
		public static ImageResolution PresetFourK = new ImageResolution(ImageResolutionType.FourK, 3840, 2160);
		public static ImageResolution PresetEightK = new ImageResolution(ImageResolutionType.EightK, 7680, 4320);

		public static ImageResolution[] imageResolutions = new ImageResolution[] {
			PresetGame, PresetFullHd, PresetFourK, PresetEightK
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

			[InspectorName("Full HD (1920 x 1080)")]
			FullHd,

			[InspectorName("4K (3840 x 2160)")]
			FourK,

			[InspectorName("8K (7680 x 4320)")]
			EightK,

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
