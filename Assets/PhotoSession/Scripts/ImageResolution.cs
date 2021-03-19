using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rowlan.PhotoSession
{
	public class ImageResolution
	{
		public static ImageResolution PresetGame = new ImageResolution(ImageResolutionType.Game, -1, -1);
		public static ImageResolution PresetFullHd = new ImageResolution(ImageResolutionType.FullHd, 1920, 1080);
		public static ImageResolution PresetFourK = new ImageResolution(ImageResolutionType.FourK, 3840, 2160);
		public static ImageResolution PresetEightK = new ImageResolution(ImageResolutionType.EightK, 7680, 4320);

		public static ImageResolution[] imageResolutions = new ImageResolution[] {
			PresetGame, PresetFullHd, PresetFourK, PresetEightK
		};

		public enum ImageResolutionType
		{
			[InspectorName("Game Screen")]
			Game,

			[InspectorName("Full HD (1920 x 1080, 16:9)")]
			FullHd,

			[InspectorName("4K (3840 x 2160, 19:9)")]
			FourK,

			[InspectorName("8K (7680 x 4320, 16:9)")]
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
		public static ImageResolution GetImageResolution(this ImageResolution.ImageResolutionType imageResolutionType)
		{
			ImageResolution imageResolution = ImageResolution.imageResolutions.ToList<ImageResolution>().Find(x => x.Type == imageResolutionType);

			if (imageResolution != null)
				return imageResolution;


			Debug.LogError("Resolution not found: " + imageResolution);

			return ImageResolution.PresetGame;
		}

	}
}
