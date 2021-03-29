using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rowlan.PhotoSession
{
    /// <summary>
    /// Simulate a flash light by having a canvas and an image. The alpha value of the image is being faded over time.
    /// </summary>
    public class CameraFlash
    {
        private float flashDuration = 0.5f;

        private Canvas canvas = null;
        private Image flashImage = null;

        private Coroutine flashCoroutine = null;

        public bool IsPlaying { get; set; }

        public CameraFlash( Canvas canvas) {

            this.canvas = canvas;

            if (canvas)
            {
                this.flashImage = canvas.GetComponentInChildren<Image>();
            }
            
        }

        public void StartCameraFlash( MonoBehaviour monoBehaviour)
        {

            if (!flashImage)
                return;

            StopCameraFlash(monoBehaviour);

            monoBehaviour.StartCoroutine(FlashCoroutine());
        }

        public void StopCameraFlash( MonoBehaviour monoBehaviour)
        {

            if (!flashImage)
                return;

            if (flashCoroutine != null)
            {
                monoBehaviour.StopCoroutine(flashCoroutine);
            }

        }

        IEnumerator FlashCoroutine()
        {
            IsPlaying = true;

            float timeElapsed = 0;

            while (timeElapsed < flashDuration)
            {
                float value = Mathf.Lerp(1, 0, timeElapsed / flashDuration);

                timeElapsed += Time.unscaledDeltaTime;

                SetFlashColorAlpha(value);

                yield return null;
            }

            StopCameraFlashNow();
        }

        public void StopCameraFlashNow() {

            SetFlashColorAlpha(0f);

            IsPlaying = false;
        }

        void SetFlashColorAlpha( float alpha) {

            if (!flashImage)
                return;

            Color color = flashImage.color;
            color.a = alpha;

            flashImage.color = color;

        }
    }
}
