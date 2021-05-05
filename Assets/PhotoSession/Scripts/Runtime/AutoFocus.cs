using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.PhotoSession
{
    public class AutoFocus : MonoBehaviour
    {
        private readonly int maxArrayLength = 144;

        public bool skipBounds = true;

        // shader has 144 predefined for the array, so max is 16x9=144
        [Range(1, 16)]
        public int focusRaysX = 4;

        // shader has 144 predefined for the array, so max is 16x9=144
        [Range(1, 9)]
        public int focusRaysY = 3;

        /// <summary>
        /// The maximum distance is used to calculate the opacity of the focus hit rectangles.
        /// </summary>
        public float maxHitDistance = 3;

        /// <summary>
        /// The material with the shader that will be used to display the focus rectangles
        /// </summary>
        public Material material;

        List<Vector3> focusScreenPoints = new List<Vector3>();

        void Start()
        {
            // initialize the float array
            material.SetInt("_ArrayLength", maxArrayLength);
            material.SetFloatArray("_PositionX", new float[maxArrayLength]);
            material.SetFloatArray("_PositionY", new float[maxArrayLength]);
            material.SetFloatArray("_Distance", new float[maxArrayLength]);
            material.SetFloat("_MaxHitDistance", maxHitDistance);
        }

        void Update()
        {
            CreateFocusPoints();

            int arrayLength = focusScreenPoints.Count;
            float[] positionX = focusScreenPoints.ConvertAll<float>(x => x.x).ToArray();
            float[] positionY = focusScreenPoints.ConvertAll<float>(x => x.y).ToArray();
            float[] distance = focusScreenPoints.ConvertAll<float>(x => x.z).ToArray();

            material.SetInt("_ArrayLength", arrayLength);
            material.SetFloatArray("_PositionX", positionX);
            material.SetFloatArray("_PositionY", positionY);
            material.SetFloatArray("_Distance", distance);
            material.SetFloat("_MaxHitDistance", maxHitDistance);

        }

        void CreateFocusPoints()
        {
            focusScreenPoints.Clear();

            int xRays = focusRaysX;
            int yRays = focusRaysY;

            if (skipBounds)
            {
                xRays++;
                yRays++;
            }
            else
            {
                xRays--;
                yRays--;
            }

            xRays = xRays <= 0 ? 1 : xRays;
            yRays = yRays <= 0 ? 1 : yRays;

            float w = Screen.width;
            float h = Screen.height;
            float dx = w / xRays;
            float dy = h / yRays;

            for (float x = 0; x <= w; x += dx)
            {
                if (skipBounds && (x <= 0 || x >= w))
                    continue;

                for (float y = 0; y <= h; y += dy)
                {
                    if (skipBounds && (y <= 0 || y >= h))
                        continue;

                    Vector3 screenPoint = new Vector3(x, y, 0);
                    Ray ray = Camera.main.ScreenPointToRay(screenPoint);

                    Debug.DrawRay(ray.origin, ray.direction * 50, UnityEngine.Color.yellow);

                    Vector3 focusScreenPoint = new Vector3(x, y, -1);

                    if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity))
                    {
                        float distance = (hit.point - ray.origin).magnitude;
                        /*
                        Vector3 vpp = Camera.main.WorldToScreenPoint(hit.point);
                        Vector3 focusHitPoint = new Vector3(vpp.x, vpp.y, distance);
                        */
                        focusScreenPoint.z = distance;
                    }

                    focusScreenPoints.Add(focusScreenPoint);
                }
            }
        }
    }
}
