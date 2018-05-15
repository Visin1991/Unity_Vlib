using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{

    public class VCamera : MonoBehaviour
    {

        Vector3 Position = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 Right = new Vector3(1.0f, 0.0f, 0.0f);
        Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);
        Vector3 Forward = new Vector3(0.0f, 0.0f, 1.0f);

        public Vector3 WorldForward = new Vector3(0.0f, 0.0f, 1.0f);
        public Vector3 WorldRight = new Vector3(1.0f, 0.0f, 0.0f);

        //Cache frustum properties
        public float NearZ = 1.0f;
        public float FarZ = 5.0f;
        [Range(0.0f, Mathf.PI)]
        public float FovY = 0.0f;
        float Aspect = 0.0f;


        float NearWindowHeight = 0.0f;
        float FarWindowHeight = 0.0f;
        bool ViewDirty = true;

        //Cache View/Proj matrices.
        Matrix4x4 View = Matrix4x4.identity;
        [HideInInspector] public Matrix4x4 Proj = Matrix4x4.identity;


        public Vector3 GetPosition()
        {
            return Position;
        }

        public void SetLens(float fovY, float aspect, float zn, float zf)
        {
            FovY = fovY;
            Aspect = aspect;
            NearZ = zn;
            FarZ = zf;
            NearWindowHeight = 2.0f * NearZ * Mathf.Tan(0.5f * FovY);
            FarWindowHeight = 2.0f * FarZ * Mathf.Tan(0.5f * FovY);
            Proj = Matrix4x4.Perspective(FovY, Aspect, NearZ, FarZ);
        }

        public float GetFovX()
        {
            float halfWidth = 0.5f * GetNearWindowWidth();
            return 2.0f * Mathf.Atan(halfWidth / NearZ);
        }

        public float GetNearWindowWidth()
        {
            return Aspect * NearWindowHeight;
        }

        float GetNearWindowHeight()
        {
            return NearWindowHeight;
        }

        float GetFarWindowWidth()
        {
            return Aspect * FarWindowHeight;
        }

        float GetFarWindowHeight()
        {
            return FarWindowHeight;
        }

        void Walk(float d)
        {
            // Position += d * Look
            Vector3 s = new Vector3(d, d, d);
            Position = Position.MultiplyAdd(s, Forward);
        }

        void Strafe(float d)
        {
            // Position += d * Right
            Vector3 s = new Vector3(d, d, d);
            Position = Position.MultiplyAdd(s, Right);
        }

        void Pitch(float angle)
        {
            Matrix4x4 R = Matrix4x4Extention.CreateRotationMatrix(Right, angle);
            Up = R.MultiplyVector(Up);
            Forward = R.MultiplyVector(Forward);
        }

        void RotateY(float angle)
        {
            // Rotate the basis vectors about the world y axis.
            Matrix4x4 R = Matrix4x4Extention.CreateRotationMatrix(Vector3.up, angle);
            Right = R.MultiplyVector(Right);
            Up = R.MultiplyVector(Up);
            Forward = R.MultiplyVector(Forward);
        }

        /// <summary>
        /// C++ code Reference
        /// float right[4]    = { 1, 0, 0, 0 };
        /// float up[4]       = { 0, 1, 0, 0 };
        /// float forward[4]  = { 0, 0, 1, 0 };
        /// float position[4] = { 0, 0, 0, 1 };
        ///
        /// float matrix[4][4] = {
        ///    {   right[0],    right[1],    right[2],    right[3] }, // First column
        ///    {      up[0],       up[1],       up[2],       up[3] }, // Second column
        ///    { forward[0],  forward[1],  forward[2],  forward[3] }, // Third column
        ///    {position[0], position[1], position[2], position[3] }  // Forth column
        /// };
        /// </summary>
        void UpdateViewMatrix()
        {
            if (ViewDirty)
            {
                Vector3 R = Right;
                Vector3 U = Up;
                Vector3 L = Forward;
                Vector3 P = Position;

                // Keep camera’s axes orthogonal to each other and of unit length
                L = L.normalized;
                U = Vector3.Normalize(Vector3.Cross(L, R));
                R = Vector3.Cross(U, L);                     // U, L already ortho-normal, so no need to normalize cross product.

                float x = -Vector3.Dot(P, R);
                float y = -Vector3.Dot(P, U);
                float z = -Vector3.Dot(P, L);

                Right = R;
                Up = U;
                Forward = L;

                View.m00 = Right.x; View.m01 = Up.x; View.m02 = Forward.x; View.m03 = 0;
                View.m10 = Right.y; View.m11 = Up.y; View.m12 = Forward.y; View.m13 = 0;
                View.m20 = Right.z; View.m21 = Up.z; View.m22 = Forward.z; View.m23 = 0;
                View.m30 = x; View.m31 = y; View.m32 = z; View.m33 = 1;

                ViewDirty = false;
            }
        }

        public void OnResize()
        {
            SetLens(0.25f * Mathf.PI, AspectRatio(), 1.0f, 50.0f);
        }

        Plane NearPlane()
        {
            return new Plane(Forward, NearZ);
        }

        Plane FarPlane()
        {
            return new Plane(Forward, FarZ);
        }

        public static float AspectRatio()
        {
            return (float)Camera.current.pixelWidth / Camera.current.pixelHeight;
        }

        private void OnDrawGizmos()
        {


            //Look = transform.localToWorldMatrix.MultiplyVector(Look);
            WorldForward = transform.localToWorldMatrix.MultiplyVector(Forward);
            WorldRight = transform.localToWorldMatrix.MultiplyVector(Right);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + WorldForward * 20.0f);

            Color32 color = Color.blue;
            color.a = 125;
            Gizmos.color = color;


            Quaternion rotation = Quaternion.LookRotation(WorldForward);
            Matrix4x4 trs = Matrix4x4.TRS(transform.position + WorldForward * FarZ, rotation, Vector3.one);
            Gizmos.matrix = trs;
            Gizmos.DrawCube(Vector3.zero, new Vector3(10.0f, 10.0f, 0.0001f));


            Vector3 topNormal = WorldForward.RotateAxis(WorldRight, 0.5f * Mathf.Rad2Deg * FovY);
            Quaternion rotation1 = Quaternion.LookRotation(topNormal);
            Matrix4x4 trs2 = Matrix4x4.TRS(transform.position, rotation1, Vector3.one);
            Gizmos.matrix = trs2;
            Gizmos.DrawCube(Vector3.zero, new Vector3(30f, 30f, 0.0001f));

            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;

        }

    }
}

