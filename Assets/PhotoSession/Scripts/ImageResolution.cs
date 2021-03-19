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
		public static ImageResolution PresetUhd = new ImageResolution(ImageResolutionType.Uhd, 3840, 2160);
		public static ImageResolution PresetUltraWide = new ImageResolution(ImageResolutionType.UltraWide, 3840, 1080);
		public static ImageResolution PresetFourK = new ImageResolution(ImageResolutionType.FourK, 4096, 2160);

		public static ImageResolution[] imageResolutions = new ImageResolution[] {
			PresetGame, PresetFullHd, PresetUhd, PresetUltraWide, PresetFourK
		};

		public enum ImageResolutionType
		{
			[InspectorName("Game Screen")]
			Game,

			[InspectorName("Full HD (1920 x 1080, 16:9)")]
			FullHd,

			[InspectorName("Ultra HD (3840 x 2160, 16:9)")]
			Uhd,

			[InspectorName("Ultra Wide (3840 x 1080, 32:9)")]
			UltraWide,

			[InspectorName("4K (4096 x 2160, 19:10)")]
			FourK,

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

			return ImageResolution.PresetGame;
		}

	}
}
