using System;
using UnityEngine;
using UnityEngine.Rendering;

#if USING_LEGACY
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Rowlan.PhotoSession.Legacy
{
    public class DepthOfFieldModule : IPhotoSessionModule
    {

#if USING_LEGACY
        private PhotoSession photoSession;
        private PostProcessVolume volume;
        private DepthOfField depthOfField;
        private DepthOfFieldSettings originalSettings;

        private bool featureActive = false;

        private bool hasTarget;
        private float hitDistance;
#endif

        public void Start(PhotoSession photoSession)
        {
#if USING_LEGACY
            this.photoSession = photoSession;

			Legacy.DepthOfFieldSettings dofSettings = photoSession.settings.legacyDepthOfFieldSettings;

            // check if user enabled the feature
            if (!dofSettings.featureEnabled)
                return;

            // get the dof profile
            featureActive = dofSettings.volume.profile.TryGetSettings(out depthOfField);
            if (!featureActive)
            {
                Debug.Log("DepthOfField enabled, but volume undefined");
                return;
            }

            this.volume = dofSettings.volume;

#endif
        }

        public void OnEnable()
        {
#if USING_LEGACY
            // backup settings
            originalSettings = new DepthOfFieldSettings(volume, depthOfField);

            // activate the DoF feature if all criteria for that are met
            if (featureActive)
            {
                volume.gameObject.SetActive(true);
                depthOfField.active = true;
            }
#endif
        }

        public void OnDisable()
        {
#if USING_LEGACY

            // restore settings (including active state)
            originalSettings.Restore(volume, depthOfField);

#endif
        }

        public void OnDrawGizmos()
        {
#if USING_LEGACY
#endif
        }


        public void Update()
        {
#if USING_LEGACY
            if (!featureActive)
                return;

            hasTarget = photoSession.autoFocusData.hasTarget;
            hitDistance = photoSession.autoFocusData.minDistance;

            UpdateFocus(hasTarget, hitDistance);
#endif
        }

        #region Private Methods and Classes

#if USING_LEGACY

        private void UpdateFocus(bool hasTarget, float hitDistance)
        {
            UpdateFocusMode(hasTarget);
            UpdateFocusSettings(hitDistance);
        }

        private void UpdateFocusMode(bool hasTarget)
        {
            if (hasTarget)
            {
                depthOfField.active = true;
            }
            else
            {
                depthOfField.active = false;
            }

        }

        private void UpdateFocusSettings(float targetDistance)
        {
            Legacy.DepthOfFieldSettings dofSettings = photoSession.settings.legacyDepthOfFieldSettings;

            depthOfField.focusDistance.overrideState = true;
            depthOfField.focusDistance.value = targetDistance + dofSettings.focusDistanceOffset;

            depthOfField.focalLength.overrideState = true;
            depthOfField.focalLength.value = dofSettings.focalLength;

            depthOfField.aperture.overrideState = true;
            depthOfField.aperture.value = dofSettings.aperture;
        }

        private class DepthOfFieldSettings
        {
            private bool volumeActive;
            private bool depthOfFieldActive;

            private FloatParameter focusDistance;
            private FloatParameter focalLength;
            private FloatParameter aperture;

            public DepthOfFieldSettings(PostProcessVolume volume, DepthOfField depthOfField)
            {

                if (!depthOfField)
                    return;

                volumeActive = volume.gameObject.activeInHierarchy;

                depthOfFieldActive = depthOfField.active; // important: don't use IsActive(), it's something different

				focusDistance = new FloatParameter
				{
					value = depthOfField.focusDistance.value,
					overrideState = depthOfField.focusDistance.overrideState
				};

				focalLength = new FloatParameter()
                {
                    value = depthOfField.focalLength.value,
                    overrideState = depthOfField.focalLength.overrideState
                };

                aperture = new FloatParameter()
                {
                    value = depthOfField.aperture.value,
                    overrideState = depthOfField.aperture.overrideState
                };


            }

            public void Restore(PostProcessVolume volume, DepthOfField depthOfField)
            {

                if (!depthOfField)
                    return;

                volume.gameObject.SetActive( volumeActive);

                depthOfField.active = depthOfFieldActive;

                depthOfField.focusDistance.overrideState = focusDistance.overrideState;
                depthOfField.focusDistance.value = focusDistance.value;

                depthOfField.focalLength.value = focalLength.value;
                depthOfField.focalLength.value = focalLength.value;

                depthOfField.aperture.overrideState = aperture.overrideState;
                depthOfField.aperture.value = aperture.value;

            }
        }
#endif
        #endregion Private Methods and Classes

    }
}