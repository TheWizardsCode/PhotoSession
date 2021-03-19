using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.PhotoSession
{
	public class Output
	{
		public enum Format
		{
			PNG,
			JPG,
			EXR,
			TGA
		}
	}

	public static class FileFormatExtension
	{
		public static string GetExtension(this Output.Format format)
		{
			switch (format)
			{
				case Output.Format.PNG: return "png";
				case Output.Format.JPG: return "jpg";
				case Output.Format.EXR: return "exr";
				case Output.Format.TGA: return "tga";
				default:
					throw new ArgumentOutOfRangeException( string.Format( "Unsupported file format {0}", format));
			}
		}

		public static byte[] Encode(this Output.Format format, Texture2D texture)
		{
			switch (format)
			{
				case Output.Format.PNG: return texture.EncodeToPNG();
				case Output.Format.JPG: return texture.EncodeToJPG();
				case Output.Format.EXR: return texture.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
				case Output.Format.TGA: return texture.EncodeToTGA();
				default:
					throw new ArgumentOutOfRangeException(string.Format("Unsupported file format {0}", format));
			}
		}

		public static TextureFormat GetTextureFormat( this Output.Format format) {
			switch (format)
			{
				case Output.Format.PNG: return TextureFormat.RGB24;
				case Output.Format.JPG: return TextureFormat.RGB24;
				case Output.Format.EXR: return TextureFormat.RGBAFloat;
				case Output.Format.TGA: return TextureFormat.RGB24;
				default:
					throw new ArgumentOutOfRangeException(string.Format("Unsupported file format {0}", format));
			}
		}
	}
}