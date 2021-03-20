using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rowlan.PhotoSession
{

	/// <summary>
	/// Create a screenshot and save it in the <see cref="ScreenshotsFolder"/>.
	/// Inside the editor the folder is parallel to the Assets folder. In build mode it's parallel to the data folder.
	/// </summary>
	public class Screenshot
	{

		private readonly string ScreenshotsFolder = "Screenshots";
		private string screenshotPath;

		/// <summary>
		/// Screenshots path is parallel to the Assets path in edit mode or parallel to the data folder in build mode
		/// </summary>
		/// <returns></returns>
		public string GetPath()
		{
			string dataPath = Application.dataPath;

			string parentPath = Path.GetDirectoryName(dataPath);

			string screenshotPath = Path.Combine(parentPath, ScreenshotsFolder);

			return screenshotPath;
		}

		/// <summary>
		/// Create a filename using pattern "<scene name> - 2021.03.16 - 16.04.22.46.png"
		/// </summary>
		/// <returns></returns>
		private string GetFilename(Output.Format format)
		{
			string sceneName = SceneManager.GetActiveScene().name;

			return string.Format("{0} - {1:yyyy.MM.dd - HH.mm.ss.ff}.{2}", sceneName, DateTime.Now, format.GetExtension());
		}

		/// <summary>
		/// Create a custom resolution screenshot using the specified camera. 
		/// The resolution can be higher than the screen size.
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void Capture( PhotoSessionSettings settings)
		{
			Camera camera = settings.photoCamera;
			int width = settings.resolution.GetImageResolution(settings.aspectRatio).Width;
			int height = settings.resolution.GetImageResolution(settings.aspectRatio).Height;
			Output.Format format = settings.outputFormat;
			bool fieldOfViewOverride = settings.fieldOfViewOverride;
			float fieldOfView = settings.fieldOfView;

			if ( width <= 0 || height <= 0)
			{
				width = Screen.width;
				height = Screen.height;
			}

			// save data which we'll modify
			RenderTexture prevRenderTexture = RenderTexture.active;
			RenderTexture prevCameraTargetTexture = camera.targetTexture;
			bool prevCameraEnabled = camera.enabled;
			float prevFieldOfView = camera.fieldOfView;

			// create rendertexture
			int msaaSamples = 1;
			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, msaaSamples);

			try
			{
				// disabling the camera is important, otherwise you get e. g. a blurry image with different focus than the one the camera displays
				// see https://docs.unity3d.com/ScriptReference/Camera.Render.html
				camera.enabled = false;

				// fov
				if (fieldOfViewOverride)
				{
					camera.fieldOfView = fieldOfView;
				}

				// set rendertexture into which the camera renders
				camera.targetTexture = renderTexture;

				// render a single frame
				camera.Render();

				// create image using the camera's render texture
				RenderTexture.active = camera.targetTexture;

				Texture2D screenShot = new Texture2D(width, height, format.GetTextureFormat(), false);
				screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
				screenShot.Apply();

				// save the image
				byte[] bytes = format.Encode( screenShot);

				string filepath = Path.Combine(this.screenshotPath, GetFilename( format));

				System.IO.File.WriteAllBytes(filepath, bytes);

				Debug.Log(string.Format("[<color=blue>Screenshot</color>]Screenshot captured\n<color=grey>{0}</color>", filepath));

			}
			catch (Exception ex)
			{
				Debug.LogError("Screenshot capture exception: " + ex);
			}
			finally
			{
				RenderTexture.ReleaseTemporary(renderTexture);

				// restore modified data
				RenderTexture.active = prevRenderTexture;
				camera.targetTexture = prevCameraTargetTexture;
				camera.enabled = prevCameraEnabled;
				camera.fieldOfView = prevFieldOfView;

			}

		}

		/// <summary>
		/// Ensure the screenshot path exists, create it if it doesn't exist yet.
		/// </summary>
		public void SetupPath()
		{
			this.screenshotPath = GetPath();

			if (!Directory.Exists(this.screenshotPath))
			{
				Directory.CreateDirectory(this.screenshotPath);
			}

		}

	}
}