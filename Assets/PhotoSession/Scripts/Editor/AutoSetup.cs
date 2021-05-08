using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.PhotoSession
{
    public class AutoSetup
    {
        private static readonly string tag_MainCamera = "MainCamera";

        private static readonly string[] script_Blacklist = {
            "SimpleCameraController", // Unity HDRP Demo
            "LookWithMouse", // Unity HDRP Demo
            "CharacterController", // Unity HDRP Demo
            "PlayerMovement", // Unity HDRP Demo
            "MFreeLookCamera", // Malbers FreeLookCameraRig
            "CameraWallStop", // Malbers FreeLookCameraRig
            };

        public static void Execute(PhotoSession photoSession)
        {

            AssignMainCamera( photoSession);

            AssignScriptBlacklist( photoSession);
        }

        private static void AssignMainCamera(PhotoSession photoSession)
        {

            // find all cameras
            Camera[] cameras = Object.FindObjectsOfType<Camera>();

            Camera mainCamera = null;
            foreach (Camera camera in cameras)
            {

                // skip all inaktive
                if (!camera.isActiveAndEnabled)
                    continue;

                // last one wins unless it's tagged as maincamera, then that one wins
                if (!mainCamera || camera.CompareTag(tag_MainCamera))
                {
                    mainCamera = camera;
                }

            }

            if (mainCamera)
            {
                photoSession.settings.photoCamera = mainCamera;
            }
        }

        private static void AssignScriptBlacklist(PhotoSession photoSession)
        {

            Camera photoCamera = photoSession.settings.photoCamera as Camera;

            if (!photoCamera)
                return;

            List<Component> components = new List<Component>();

            foreach (string scriptName in script_Blacklist)
            {

                // recursively search the blacklisted objects from the photo camera gameobject up to the root
                Transform transform = photoCamera.transform;
                while (transform)
                {

                    Component component = transform.gameObject.GetComponent(scriptName);

                    if (component)
                        components.Add(component);

                    transform = transform.parent;
                }
            }

            // set the disabled components in the serializedObject
            photoSession.settings.disabledComponents = components.ToArray();
		}
    }
}