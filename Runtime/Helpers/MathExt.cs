using System;
using UnityEngine;
using static traVRsal.SDK.BasicEntity;
using Random = UnityEngine.Random;

namespace traVRsal.SDK
{
    public static class MathExt
    {
        public static float Cm2Inch(float value)
        {
            return value * 0.393701f;
        }

        public static float Meters2Feet(float value)
        {
            return value * 3.28084f;
        }

        public static float Inch2Cm(float value)
        {
            return value * 2.54f;
        }

        public static Vector2 xy(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector2 xz(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector2 yz(this Vector3 vector)
        {
            return new Vector2(vector.y, vector.z);
        }

        public static Vector2 yx(this Vector3 vector)
        {
            return new Vector2(vector.y, vector.x);
        }

        public static Vector2 zx(this Vector3 vector)
        {
            return new Vector2(vector.z, vector.x);
        }

        public static Vector2 zy(this Vector3 vector)
        {
            return new Vector2(vector.z, vector.y);
        }

        public static Vector2Int Abs(this Vector2Int input)
        {
            return new Vector2Int(Mathf.Abs(input.x), Mathf.Abs(input.y));
        }

        public static Vector3 WithX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 WithY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 WithZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector2 WithX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }

        public static Vector2 WithY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }

        public static Vector2Int WithX(this Vector2Int v, int x)
        {
            return new Vector2Int(x, v.y);
        }

        public static Vector2Int WithY(this Vector2Int v, int y)
        {
            return new Vector2Int(v.x, y);
        }

        public static Vector3 WithAddX(this Vector3 v, float x)
        {
            return new Vector3(v.x + x, v.y, v.z);
        }

        public static Vector3 WithAddY(this Vector3 v, float y)
        {
            return new Vector3(v.x, v.y + y, v.z);
        }

        public static Vector3 WithAddZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, v.z + z);
        }

        public static bool ApproximatelyEqual(this Vector3 v, Vector3 v2)
        {
            return Mathf.Approximately(v.x, v2.x) && Mathf.Approximately(v.y, v2.y) && Mathf.Approximately(v.z, v2.z);
        }

        public static bool ApproximatelyEqual(this Vector3 v, Vector3 v2, float tolerance)
        {
            return Approximately(v.x, v2.x, tolerance) && Approximately(v.y, v2.y, tolerance) && Approximately(v.z, v2.z, tolerance);
        }

        public static bool Approximately(this float number, float other, float tolerance)
        {
            return Mathf.Abs(number - other) <= tolerance;
        }

        public static Vector2 WithAddX(this Vector2 v, float x)
        {
            return new Vector2(v.x + x, v.y);
        }

        public static Vector2 WithAddY(this Vector2 v, float y)
        {
            return new Vector2(v.x, v.y + y);
        }

        public static Vector2Int WithAddX(this Vector2Int v, int x)
        {
            return new Vector2Int(v.x + x, v.y);
        }

        public static Vector2Int WithAddY(this Vector2Int v, int y)
        {
            return new Vector2Int(v.x, v.y + y);
        }

        public static Vector2[] Vector3Arr2Vector2(Vector3[] vector3Arr)
        {
            Vector2[] result = new Vector2[vector3Arr.Length];

            for (int i = 0; i < vector3Arr.Length; i++)
            {
                result[i] = new Vector2(vector3Arr[i].x, vector3Arr[i].z);
            }

            return result;
        }

        public static bool ApproximatelyEqual(this Vector2 v, Vector2 v2)
        {
            return Mathf.Approximately(v.x, v2.x) && Mathf.Approximately(v.y, v2.y);
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            return Quaternion.Euler(0, 0, degrees) * v;
        }

        public static Vector2Int ToInt(this Vector2 v)
        {
            return new Vector2Int((int) v.x, (int) v.y);
        }

        public static int GetScaled(int original, int max, int originalMax)
        {
            return Mathf.FloorToInt((float) original / (float) originalMax * (float) max);
        }

        public static bool IsBetween(this Vector3 C, Vector3 A, Vector3 B)
        {
            return Vector3.Dot((B - A).normalized, (C - B).normalized) < 0f && Vector3.Dot((A - B).normalized, (C - A).normalized) < 0f;
        }

        public static Vector3 AsVector3(this Vector2Int position)
        {
            return new Vector3(position.x, 0, position.y);
        }

        public static int ParseInt(string source, int defaultValue = -1)
        {
            return !int.TryParse(source, out int result) ? defaultValue : result;
        }

        public static float ParseFloat(string source, float defaultValue = -1)
        {
            return !float.TryParse(source, out float result) ? defaultValue : result;
        }

        public static Vector2Int RotatePoint(Vector2Int pointToRotate, Vector2Int dimensions)
        {
            return new Vector2Int(pointToRotate.y, -pointToRotate.x + dimensions.y - 1);
        }

        public static Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, float angleInDegrees)
        {
            float angleInRadians = angleInDegrees * (Mathf.PI / 180);
            float cosTheta = Mathf.Cos(angleInRadians);
            float sinTheta = Mathf.Sin(angleInRadians);
            return new Vector2(
                (cosTheta * (pointToRotate.x - centerPoint.x) - sinTheta * (pointToRotate.y - centerPoint.y) + centerPoint.x),
                (sinTheta * (pointToRotate.x - centerPoint.x) + cosTheta * (pointToRotate.y - centerPoint.x) + centerPoint.y)
            );
        }

        public static Vector3 Abs(this Vector3 v)
        {
            v.x = Mathf.Abs(v.x);
            v.y = Mathf.Abs(v.y);
            v.z = Mathf.Abs(v.z);

            return v;
        }

        public static Vector3 GetRandomVector3(Vector3 range)
        {
            return new Vector3(Random.Range(-range.x / 2f, range.x / 2f), Random.Range(-range.y / 2f, range.y / 2f), Random.Range(-range.z / 2f, range.z / 2f));
        }

        public static Direction GetDirection(Vector2 start, Vector2 end)
        {
            float x = Mathf.Abs(start.x - end.x);
            float y = Mathf.Abs(start.y - end.y);

            if (x > y) return end.x - start.x > 0 ? Direction.East : Direction.West;
            if (x < y) return end.y - start.y > 0 ? Direction.North : Direction.South;

            return Direction.None;
        }

        /*  General case solution for a rectangle
 *
 *  https://stackoverflow.com/questions/13002979/how-to-calculate-rotation-angle-from-rectangle-points
 *  
 *  Given coordinates of [x1, y1, x2, y2, x3, y3, x4, y4]
 *  where the corners are:
 *            top left    : x1, y1
 *            top right   : x2, y2
 *            bottom right: x3, y3
 *            bottom left : x4, y4
 *
 *  The centre is the average top left and bottom right coords:
 *  center: (x1 + x3) / 2 and (y1 + y3) / 2
 *
 *  Clockwise rotation: Math.atan((x1 - x4)/(y1 - y4)) with
 *  adjustment for the quadrant the angle is in.
 *
 *  Note that if using page coordinates, y is +ve down the page which
 *  is the reverse of the mathematics sense so y page coordinates
 *  should be multiplied by -1 before being given to the function.
 *  (e.g. a page y of 400 should be -400).
 */
        public static float GetRectangleRotation(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            // Get differences top left minus bottom left
            Vector2 diffs = new Vector2(x1 - x4, y1 - y4);
            if (diffs.y == 0) return 0;

            // Get rotation in degrees
            float rotation = Mathf.Atan(diffs.x / diffs.y) * 180 / Mathf.PI;

            // Adjust for 2nd & 3rd quadrants, i.e. diff y is -ve.
            if (diffs.y < 0)
            {
                rotation += 180;

                // Adjust for 4th quadrant
                // i.e. diff x is -ve, diff y is +ve
            }
            else if (diffs.x < 0)
            {
                rotation += 360;
            }

            return rotation;
        }

        public static Vector2 GetRectangleCenter(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            return new Vector2((x1 + x3) / 2, (y1 + y3) / 2);
        }

        // Return True if the point is in the polygon.
        public static bool PointInsidePolygon(Vector2[] points, float x, float y)
        {
            // Get the angle between the point and the
            // first and last vertices.
            int maxPoint = points.Length - 1;
            float totalAngle = GetAngle(
                points[maxPoint].x, points[maxPoint].y,
                x, y,
                points[0].x, points[0].y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < maxPoint; i++)
            {
                totalAngle += GetAngle(
                    points[i].x, points[i].y,
                    x, y,
                    points[i + 1].x, points[i + 1].y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            // The following statement was changed. See the comments.
            //return (Math.Abs(total_angle) > 0.000001);
            return Math.Abs(totalAngle) > 1;
        }

        // Return the angle ABC.
        // Return a value between PI and -PI.
        // Note that the value is the opposite of what you might
        // expect because Y coordinates increase downward.
        private static float GetAngle(float Ax, float Ay, float Bx, float By, float Cx, float Cy)
        {
            // Get the dot product.
            float dotProduct = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            float crossProduct = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return (float) Math.Atan2(crossProduct, dotProduct);
        }

        // Return the dot product AB · BC.
        // Note that AB · BC = |AB| * |BC| * Cos(theta).
        private static float DotProduct(float Ax, float Ay, float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the dot product
            return BAx * BCx + BAy * BCy;
        }

        // Return the cross product AB x BC.
        // The cross product is a vector perpendicular to AB
        // and BC having length |AB| * |BC| * Sin(theta) and
        // with direction given by the right-hand rule.
        // For two vectors in the X-Y plane, the result is a
        // vector with X and Y components 0 so the Z component
        // gives the vector's length and direction.
        private static float CrossProductLength(float Ax, float Ay, float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return BAx * BCy - BAy * BCx;
        }
    }
}