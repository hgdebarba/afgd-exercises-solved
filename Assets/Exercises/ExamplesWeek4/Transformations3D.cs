using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AfGD
{

    // In this example we replicate the features of the Transform component using matrices
    public class Transformations3D : MonoBehaviour
    {
        // Variables to input the transformations
        public float translationX = 0;
        public float translationY = 0;
        public float translationZ = 0;

        public float angleX = 0;
        public float angleY = 0;
        public float angleZ = 0;

        public float scaleX = 1;
        public float scaleY = 1;
        public float scaleZ = 1;

        // Store the full transformation
        Matrix4x4 transformation = Matrix4x4.identity;

        // Called when script is loaded or a value is changed in the inspector 
        private void OnValidate()
        {
            // create a rotation, translation and scale matrices
            // rotation around the z axis
            Matrix4x4 Rz = Matrix4x4.identity;
            float angleRad = Mathf.Deg2Rad * angleZ;
            Rz.m00 = Mathf.Cos(angleRad);
            Rz.m10 = Mathf.Sin(angleRad);
            Rz.m01 = -Mathf.Sin(angleRad);
            Rz.m11 = Mathf.Cos(angleRad);

            // rotation around the y axis
            Matrix4x4 Ry = Matrix4x4.identity;
            angleRad = Mathf.Deg2Rad * angleY;
            Ry.m00 = Mathf.Cos(angleRad);
            Ry.m20 = -Mathf.Sin(angleRad);
            Ry.m02 = Mathf.Sin(angleRad);
            Ry.m22 = Mathf.Cos(angleRad);

            // rotation around the x axis
            Matrix4x4 Rx = Matrix4x4.identity;
            angleRad = Mathf.Deg2Rad * angleX;
            Rx.m11 = Mathf.Cos(angleRad);
            Rx.m21 = Mathf.Sin(angleRad);
            Rx.m12 = -Mathf.Sin(angleRad);
            Rx.m22 = Mathf.Cos(angleRad);

            // 3D translation is the last column of the 4x4 matrix
            Matrix4x4 T = Matrix4x4.identity;
            T.m03 = translationX;
            T.m13 = translationY;
            T.m23 = translationZ;

            // scale is the main diagonal of the matrix
            Matrix4x4 S = Matrix4x4.identity;
            S.m00 = scaleX;
            S.m11 = scaleY;
            S.m22 = scaleZ;

            // we concatenate the transformations into a single matrix
            // notice that, since we use column vectors, 
            // transformations are applied from right to left
            // that means first Scale -> then Rotation -> and Translation last
            transformation = T * Ry * Rx * Rz * S;
            // the order of rotation with euler angles is arbitrary, 
            // here I immitate the the convention used in Unity, with
            // rotation around z -> then around x -> and around y last
            // this guarantees that the transformation we apply here will match that
            // of the Transform component of regular game objects in an Unity scene
        }

        // Do the visualization of out transformation
        void OnDrawGizmos()
        {

            // we apply the transformation, now all our Gizmos.Draw... calls will use this matrix
            Gizmos.matrix = transformation;

            // draw a cube, with a wire frame, with the transformation that we defined above
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector3(0, 0, 0), Vector3.one * 1.005f);
            Gizmos.color = Color.grey * 0.25f;
            Gizmos.DrawCube(new Vector3(0, 0, 0), Vector3.one);

            // we pop the transformation matrix and replace it with the identity matrix
            // that is the matrix that will not affect other matrices or points/vectors
            // we don't need it in this example, but it is a good idea to avoid hard to detect
            // bugs if we modify the scene later
            Gizmos.matrix = Matrix4x4.identity;

        }
    }
}