using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ScreenCapture;

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

		public int SuperSize { get; set; } = 1;
		public bool StereoEnabled { get; set; } = false;
		public StereoScreenCaptureMode StereoScreenCaptureMode { get; set; } = StereoScreenCaptureMode.BothEyes;

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
        private string GetFilename()
        {
            string sceneName = SceneManager.GetActiveScene().name;

            return string.Format("{0} - {1:yyyy.MM.dd - HH.mm.ss.ff}.png", sceneName, DateTime.Now);
        }

        /// <summary>
        /// Create a screenshot and save it in <see cref="screenshotPath"/>.
        /// </summary>
        public void Capture()
        {
            string filepath = Path.Combine(this.screenshotPath, GetFilename());

            if(StereoEnabled) 
            {
                ScreenCapture.CaptureScreenshot(filepath, this.StereoScreenCaptureMode);
            }
            else 
            {
                ScreenCapture.CaptureScreenshot(filepath, this.SuperSize);
            }

            Debug.Log(string.Format("[<color=blue>Screenshot</color>]Screenshot captured\n<color=grey>{0}</color>", filepath));
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