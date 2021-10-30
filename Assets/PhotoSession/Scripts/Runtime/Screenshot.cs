using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
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
        private PhotoSession photoSession;

        public Screenshot(PhotoSession photoSession)
        {
            this.photoSession = photoSession;
        }

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

		private void WriteToFile(PhotoSessionSettings settings, RenderTexture outputTexture)
		{
			Output.Format format = settings.outputFormat;

			// create image using the camera's render texture
			RenderTexture.active = outputTexture;

			Texture2D screenShot = new Texture2D(outputTexture.width, outputTexture.height, settings.outputFormat.GetTextureFormat(), false);
			screenShot.ReadPixels(new Rect(0, 0, outputTexture.width, outputTexture.height), 0, 0);
			screenShot.Apply();

			// save the image
			byte[] bytes = format.Encode( screenShot);

			// destroy the texture
			UnityEngine.Object.Destroy(screenShot);
			screenShot = null;

			string filepath = Path.Combine(this.screenshotPath, GetFilename( format));

			System.IO.File.WriteAllBytes(filepath, bytes);

			Debug.Log(string.Format("[<color=blue>Screenshot</color>]Screenshot captured\n<color=grey>{0}</color>", filepath));
		}

		private void CaptureFlat(PhotoSessionSettings settings)
		{
			if (!settings.photoCamera)
            {
				AutoSetup.Execute(photoSession);
            }
			Camera camera = settings.photoCamera;

			int width = settings.resolution.GetImageResolution(settings.aspectRatio).Width;
			int height = settings.resolution.GetImageResolution(settings.aspectRatio).Height;
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

				WriteToFile(settings, renderTexture);
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

		private void Capture360(PhotoSessionSettings settings)
		{
			int width = settings.resolution.GetImageResolution(settings.aspectRatio).Width;
			int height = settings.resolution.GetImageResolution(settings.aspectRatio).Height;

			if ( width <= 0 || height <= 0)
			{
				width = Screen.width;
				height = Screen.height;
			}

			int max = Mathf.Max(width, height);
			int size = (int) Mathf.Pow(2, Mathf.Ceil(Mathf.Log(max)/Mathf.Log(2)));
			int msaaSamples = 8;
			Debug.Log("Size: " + size);
			RenderTexture cubemap = RenderTexture.GetTemporary(size, size, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, msaaSamples);
			Debug.Log("Cubemap size: " + cubemap.width + "x" + cubemap.height);
			cubemap.dimension = TextureDimension.Cube;
			RenderTexture equirect = RenderTexture.GetTemporary(size * 2, size, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, msaaSamples);
			settings.photoCamera.RenderToCubemap(cubemap);
			cubemap.ConvertToEquirect(equirect);
			
			WriteToFile(settings, equirect);
		}

		/// <summary>
		/// Create a custom resolution screenshot using the specified camera. 
		/// The resolution can be higher than the screen size.
		/// </summary>
		/// <param name="settings"></param>
		public void Capture( PhotoSessionSettings settings)
		{
			switch (settings.photoType)
			{
				case PhotoSessionSettings.PhotoType.Flat:
					CaptureFlat(settings);
					break;
				case PhotoSessionSettings.PhotoType.Mono360:
					Capture360(settings);
					break;
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