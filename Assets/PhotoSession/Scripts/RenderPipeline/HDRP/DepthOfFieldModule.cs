using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if USING_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif

namespace Rowlan.PhotoSession.Hdrp
{
    public class DepthOfFieldModule : IPhotoSessionModule
    {

#if USING_HDRP
        private PhotoSession photoSession;
        private Volume volume;
        private DepthOfField depthOfField;
        private DepthOfFieldSettings originalSettings;

        private bool featureActive = false;

        private DepthOfFieldMode defaultDepthOfFieldMode = DepthOfFieldMode.Manual;

        private Ray ray;
        private RaycastHit hit;
        private float hitDistance;
#endif

        public void Start(PhotoSession photoSession)
        {
#if USING_HDRP
            this.photoSession = photoSession;

            HdrpDepthOfFieldSettings hdrpSettings = photoSession.settings.hdrpDepthOfFieldSettings;

            // check if user enabled the feature
            if (!hdrpSettings.featureEnabled)
                return;

            // get the dof profile
            featureActive = hdrpSettings.volume.profile.TryGet(out depthOfField);
            if (!featureActive)
            {
                Debug.Log("HDRP DepthOfField enabled, but volume undefined");
                return;
            }

            this.volume = hdrpSettings.volume;

#endif
        }

        public void OnEnable()
        {
#if USING_HDRP
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
#if USING_HDRP

            // restore settings (including active state)
            originalSettings.Restore(volume, depthOfField);

#endif
        }

        public void OnDrawGizmos()
        {
#if USING_HDRP
#endif
        }


        public void Update()
        {
#if USING_HDRP
            if (!featureActive)
                return;

            ray = new Ray(photoSession.settings.photoCamera.transform.position, photoSession.settings.photoCamera.transform.forward * photoSession.settings.hdrpDepthOfFieldSettings.maxFocusDistance);
            bool hasTarget = Physics.Raycast(ray, out hit, photoSession.settings.hdrpDepthOfFieldSettings.maxFocusDistance);
            if (hasTarget)
            {
                hitDistance = Vector3.Distance(photoSession.settings.photoCamera.transform.position, hit.point);
            }

            UpdateFocus(hasTarget, hitDistance);
#endif
        }

        #region Private Methods and Classes

#if USING_HDRP

        private void UpdateFocus(bool hasTarget, float hitDistance)
        {
            UpdateFocusMode(hasTarget);
            UpdateFocusSettings(hitDistance);
        }

        private void UpdateFocusMode(bool hasTarget)
        {
            if (hasTarget)
            {
                depthOfField.focusMode.overrideState = true;
                depthOfField.focusMode.value = defaultDepthOfFieldMode;
            }
            else
            {
                depthOfField.focusMode.overrideState = true;
                depthOfField.focusMode.value = DepthOfFieldMode.Off;
            }

        }

        private void UpdateFocusSettings(float targetDistance)
        {
            HdrpDepthOfFieldSettings hdrpSettings = photoSession.settings.hdrpDepthOfFieldSettings;

            switch (depthOfField.focusMode.value)
            {
                case DepthOfFieldMode.Off:
                    // nothing to do
                    break;

                case DepthOfFieldMode.UsePhysicalCamera:
                    depthOfField.focusDistance.overrideState = true;
                    depthOfField.focusDistance.value = targetDistance;
                    break;

                case DepthOfFieldMode.Manual:
                    depthOfField.nearFocusStart.overrideState = true;
                    depthOfField.nearFocusStart.value = 0f;
                    depthOfField.nearFocusEnd.overrideState = true;
                    depthOfField.nearFocusEnd.value = targetDistance + hdrpSettings.hitDistanceNearFocusEndOffset;

                    depthOfField.farFocusStart.overrideState = true;
                    depthOfField.farFocusStart.value = targetDistance + hdrpSettings.hitDistanceFarFocusStartOffset;
                    depthOfField.farFocusEnd.overrideState = true;
                    depthOfField.farFocusEnd.value = depthOfField.farFocusStart.value + hdrpSettings.hitDistanceFarFocusEndOffset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unsupported enum " + depthOfField.focusMode.value);
            }
        }

        private class DepthOfFieldSettings
        {
            private bool volumeActive;
            private bool depthOfFieldActive;

            private DepthOfFieldModeParameter focusMode;

            private FloatParameter nearFocusStart;
            private FloatParameter nearFocusEnd;

            private FloatParameter farFocusStart;
            private FloatParameter farFocusEnd;

            private FloatParameter focusDistance;

            public DepthOfFieldSettings(Volume volume, DepthOfField depthOfField)
            {

                if (!depthOfField)
                    return;

                volumeActive = volume.gameObject.activeInHierarchy;

                depthOfFieldActive = depthOfField.active; // important: don't use IsActive(), it's something different

                focusMode = new DepthOfFieldModeParameter(depthOfField.focusMode.value, depthOfField.focusMode.overrideState);

                focusDistance = new FloatParameter(depthOfField.focusDistance.value, depthOfField.focusDistance.overrideState);

                nearFocusStart = new FloatParameter(depthOfField.nearFocusStart.value, depthOfField.nearFocusStart.overrideState);
                nearFocusEnd = new FloatParameter(depthOfField.nearFocusEnd.value, depthOfField.nearFocusEnd.overrideState);

                farFocusStart = new FloatParameter(depthOfField.farFocusStart.value, depthOfField.farFocusStart.overrideState);
                farFocusEnd = new FloatParameter(depthOfField.farFocusEnd.value, depthOfField.farFocusEnd.overrideState);

            }

            public void Restore(Volume volume, DepthOfField depthOfField)
            {

                if (!depthOfField)
                    return;

                volume.gameObject.SetActive( volumeActive);

                depthOfField.active = depthOfFieldActive;

                depthOfField.focusMode.overrideState = focusMode.overrideState;
                depthOfField.focusMode.value = focusMode.value;

                depthOfField.nearFocusStart.overrideState = nearFocusStart.overrideState;
                depthOfField.nearFocusStart.value = nearFocusStart.value;

                depthOfField.nearFocusEnd.overrideState = nearFocusEnd.overrideState;
                depthOfField.nearFocusEnd.value = nearFocusEnd.value;

                depthOfField.farFocusStart.overrideState = farFocusStart.overrideState;
                depthOfField.farFocusStart.value = farFocusStart.value;

                depthOfField.farFocusEnd.overrideState = farFocusEnd.overrideState;
                depthOfField.farFocusEnd.value = farFocusEnd.value;

                depthOfField.focusDistance.overrideState = focusDistance.overrideState;
                depthOfField.focusDistance.value = focusDistance.value;
            }
        }
#endif
        #endregion Private Methods and Classes

    }
}