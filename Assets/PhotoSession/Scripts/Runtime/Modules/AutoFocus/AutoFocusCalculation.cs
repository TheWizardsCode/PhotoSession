using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.PhotoSession
{
    public class AutoFocusCalculation
    {
        /// <summary>
        /// Calculate the screen and focus points, distance, etc
        /// </summary>
        /// <param name="data"></param>
        public static void UpdateOutputData(AutoFocusInput input, Vector2 point, ref AutoFocusData data)
        {
            data.Reset();
            data.maxRayLength = input.maxRayLength;

            Vector3 screenPoint = new Vector3(point.x, point.y, 0);
            Ray ray = Camera.main.ScreenPointToRay(screenPoint);

            Vector3 focusScreenPoint = new Vector3(screenPoint.x, screenPoint.y, -1);

            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity))
            {
                float distance = (hit.point - ray.origin).magnitude;
                focusScreenPoint.z = distance;

                data.hasTarget = true;
            }

            data.screenPoints.Add(focusScreenPoint);

            data.Calculate();
        }

        /// <summary>
        /// Calculate the screen and focus points, distance, etc
        /// </summary>
        /// <param name="data"></param>
        public static void UpdateOutputData(AutoFocusInput input, ref AutoFocusData data)
        {
            data.Reset();
            data.maxRayLength = input.maxRayLength;

            // set number of rays for x and y coordinates
            int xRays = input.focusRays.x;
            int yRays = input.focusRays.y;

            if (input.skipBounds)
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

            for (int xr = 0; xr <= xRays; xr++)
            {
                float x = xr * dx;

                if (input.skipBounds && (x <= 0 || x >= w))
                    continue;

                for (int yr = 0; yr <= yRays; yr++)
                {
                    float y = yr * dy;

                    if (input.skipBounds && (y <= 0 || y >= h))
                        continue;

                    Vector3 screenPoint = new Vector3(x, y, 0);
                    Ray ray = Camera.main.ScreenPointToRay(screenPoint);

                    //if (debug)
                    //{
                    //    Debug.DrawRay(ray.origin, ray.direction * 50, UnityEngine.Color.yellow);
                    //}

                    Vector3 focusScreenPoint = new Vector3(x, y, -1);

                    if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity))
                    {
                        float distance = (hit.point - ray.origin).magnitude;
                        /*
                        Vector3 vpp = Camera.main.WorldToScreenPoint(hit.point);
                        Vector3 focusHitPoint = new Vector3(vpp.x, vpp.y, distance);
                        */
                        focusScreenPoint.z = distance;

                        data.hasTarget = true;
                    }

                    data.screenPoints.Add(focusScreenPoint);
                }
            }

            data.Calculate();
        }
    }
}