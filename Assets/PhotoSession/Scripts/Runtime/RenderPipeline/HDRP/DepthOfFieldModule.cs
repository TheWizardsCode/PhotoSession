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

        private bool hasTarget;
        private float hitDistance;
#endif

        public void Start(PhotoSession photoSession)
        {
#if USING_HDRP
            this.photoSession = photoSession;

            Hdrp.DepthOfFieldSettings dofSettings = photoSession.settings.hdrpDepthOfFieldSettings;

            // check if user enabled the feature
            if (!dofSettings.featureEnabled)
                return;

            // get the dof profile
            featureActive = dofSettings.volume.profile.TryGet(out depthOfField);
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

            hasTarget = photoSession.autoFocusData.hasTarget;
            hitDistance = photoSession.autoFocusData.minDistance;

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
            Hdrp.DepthOfFieldSettings dofSettings = photoSession.settings.hdrpDepthOfFieldSettings;

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
                    depthOfField.nearFocusEnd.value = targetDistance + dofSettings.hitDistanceNearFocusEndOffset;

                    depthOfField.farFocusStart.overrideState = true;
                    depthOfField.farFocusStart.value = targetDistance + dofSettings.hitDistanceFarFocusStartOffset;
                    depthOfField.farFocusEnd.overrideState = true;
                    depthOfField.farFocusEnd.value = depthOfField.farFocusStart.value + dofSettings.hitDistanceFarFocusEndOffset;
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