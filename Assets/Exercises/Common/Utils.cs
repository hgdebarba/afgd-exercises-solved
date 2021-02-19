using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD
{
    public static class Extensions
    {
        /// <summary>
        /// Unity supports Matrix4x4 * Vector4 operations (aka postmultiplication of a vector by a matrix), 
        /// but not Vector4 * Matrix4x4 (aka premultiplication of a vector by a matrix).
        /// Let's extend the Vector4 class to support it.
        /// Unfortunatelly, we cannot do operator overloading when extending a class
        /// https://stackoverflow.com/questions/172658/operator-overloading-with-c-sharp-extension-methods
        /// so we do the Vector4-Matrix4x4 multiplication possible with a function, to use it: myVec4.Mult(myMat4);
        /// </summary>
        /// <param name="v"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector4 Mult(this Vector4 v, Matrix4x4 m)
        {
            Vector4 r = v;
            // we interpret the vector as a row vector (1x4) to multiply it by the matrix
            // matrix multiplication can be implemented with the dot product operation
            r[0] = Vector4.Dot(v, m.GetColumn(0));
            r[1] = Vector4.Dot(v, m.GetColumn(1));
            r[2] = Vector4.Dot(v, m.GetColumn(2));
            r[3] = Vector4.Dot(v, m.GetColumn(3));
            return r;
        }

        /// <summary>
        /// decompose matrix into translation, rotation and scale. Doesn't work for negative scale factors.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public static void GetTRS(this Matrix4x4 value, out Vector3 position, out Quaternion rotation, out Vector3 scale)
        {
            Matrix4x4 m = value;
            // decompose a 4x4 transformation matrix into position, rotation and scale
            // translation
            position = m.GetColumn(3);

            // scale
            scale = Vector3.zero;
            scale.x = m.GetColumn(0).magnitude;
            scale.y = m.GetColumn(1).magnitude;
            scale.z = m.GetColumn(2).magnitude;

            // we need to normalize the columns of the matrix to extract rotation
            m.SetColumn(0, m.GetColumn(0) / scale.x);
            m.SetColumn(1, m.GetColumn(1) / scale.y);
            m.SetColumn(2, m.GetColumn(2) / scale.z);

            // rotation (considerably more complicated)
            rotation = Quaternion.identity;
            float ww = 0.25f * (m.m00 + m.m11 + m.m22 + 1);
            float epsilon = 0.00001f;
            if (ww >= epsilon)
            {
                rotation.w = Mathf.Sqrt(ww);
                float oneOver4w = 1.0f / (4.0f * rotation.w);
                rotation.x = (m.m21 - m.m12) * oneOver4w;
                rotation.y = (m.m02 - m.m20) * oneOver4w;
                rotation.z = (m.m10 - m.m01) * oneOver4w;
            }
            else
            {
                rotation.w = 0;
                float xx = -0.5f * (m.m11 + m.m22);
                if (xx > epsilon)
                {
                    rotation.x = Mathf.Sqrt(xx);
                    float oneOver2x = 1.0f / (2.0f * rotation.x);
                    rotation.y = m.m01 * oneOver2x;
                    rotation.z = m.m02 * oneOver2x;
                }
                else
                {
                    rotation.x = 0;
                    float yy = 0.5f * (1.0f - m.m22);
                    if (yy > epsilon)
                    {
                        rotation.y = Mathf.Sqrt(yy);
                        rotation.z = m.m12 / (2.0f * rotation.y);
                    }
                    else
                    {
                        rotation.y = 0;
                        rotation.z = 1;
                    }
                }
            }
        }

    }

    public static class Helper
    {
        public static Transform FindInHierarchy(this Transform tr, string name)
        {
            Transform returnTr = tr.Find(name);
            if (returnTr == null)
            {
                for (int i = 0; i < tr.childCount; i++)
                {
                    returnTr = tr.GetChild(i).FindInHierarchy(name);
                    if (returnTr != null)
                        break;
                }
            }
            return returnTr;
        }
    }

    public static class DebugDraw
    {


        /// <summary>
        /// Draw frame of reference
        /// </summary>
        /// <param name="P"></param>
        /// <param name="size"></param>
        public static void DrawFrame(Matrix4x4 P, float size = 0.1f)
        {
            Vector3 center = P.GetColumn(3);
            Quaternion rot = P.rotation;
            Debug.DrawLine(center, center + rot * Vector3.right * size, Color.red);
            Debug.DrawLine(center, center + rot * Vector3.up * size, Color.green);
            Debug.DrawLine(center, center + rot * Vector3.forward * size, Color.blue);
        }


        /// <summary>
        /// Draw bone connecting transforms (not very pretty)
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="trParent"></param>
        public static void DrawBone(Matrix4x4 tr, Matrix4x4 trParent)
        {
            Vector3 posP = tr.GetColumn(3);
            Vector3 posPlocal = trParent.GetColumn(3);
            Vector3 vec = posPlocal - posP;
            float scale = vec.magnitude;
            Vector3 p1 = new Vector3(0.05f, 0.15f, 0.05f);
            Vector3 p2 = new Vector3(0.05f, 0.15f, -0.05f);
            Vector3 p3 = new Vector3(-0.05f, 0.15f, -0.05f);
            Vector3 p4 = new Vector3(-0.05f, 0.15f, 0.05f);

            p1 = trParent.MultiplyPoint3x4(p1 * scale);
            p2 = trParent.MultiplyPoint3x4(p2 * scale);
            p3 = trParent.MultiplyPoint3x4(p3 * scale);
            p4 = trParent.MultiplyPoint3x4(p4 * scale);
            vec *= 0.1f;

            Debug.DrawLine(posP, p1, Color.cyan);
            Debug.DrawLine(posP, p2, Color.cyan);
            Debug.DrawLine(posP, p3, Color.cyan);
            Debug.DrawLine(posP, p4, Color.cyan);
            Debug.DrawLine(posPlocal, p1, Color.cyan);
            Debug.DrawLine(posPlocal, p2, Color.cyan);
            Debug.DrawLine(posPlocal, p3, Color.cyan);
            Debug.DrawLine(posPlocal, p4, Color.cyan);

            Debug.DrawLine(p1, p2, Color.cyan);
            Debug.DrawLine(p2, p3, Color.cyan);
            Debug.DrawLine(p3, p4, Color.cyan);
            Debug.DrawLine(p4, p1, Color.cyan);
        }



    }
}
