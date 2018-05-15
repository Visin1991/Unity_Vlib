using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace V
{
    public class ThridPersonCamera : MonoBehaviour, PlayerCamera
    {
        CameraType cameraType = CameraType.ThridPerson;
        public CameraType CameraType { get { return cameraType; } }
        public Transform Transform { get { return transform; } }

        public CameraRotateModel cameraRotateModel = CameraRotateModel.Free;

        public KeyCode controlKey;

        [SerializeField] private Vector2 pitchMinMax = new Vector2(0, 85);
        [SerializeField] private float rotationSmoothTime = 0.5f;
        private Vector3 rotationSmoothVelocity;
        private Vector3 currentRotation;

        [SerializeField] private Transform target;
        [SerializeField] private Vector2 rangeToTarget = new Vector2(2, 10);
        [SerializeField] private float cameraMoveSensitivity = 10;

        public float dstToTarget = 10;

        private float yaw;  //Rotation around Y Axis
        public float pitch = 55;//Rotation around X Axis
        public bool isFixedYaw = true;

        //RaycastHit hitInfo = new RaycastHit();
        //float blockTime = 0.0f;
        //public float adjustTime = 1.5f;

        public void InitialCamera()
        {
           
        }

        public void UpdateCamera()
        {

        }

        

        public void LateUpdateCamera()
        {
            //Add detal Camera Value each frame
            yaw += detal_yaw * cameraMoveSensitivity;
            pitch += detal_pitch * cameraMoveSensitivity;
            dstToTarget += detal_dstToTarget * cameraMoveSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            dstToTarget = Mathf.Clamp(dstToTarget, rangeToTarget.x, rangeToTarget.y);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw,0), ref rotationSmoothVelocity, rotationSmoothTime);
            transform.eulerAngles = currentRotation;

            //Reset detal value to zero each frame
            SetCameraDetal(0,0,0);
        }

        public void SetCameraDetal(float dy, float dp, float dd)
        {
            detal_yaw = dy; detal_pitch = dp; detal_dstToTarget = dd;
        }

        public float detal_yaw;
        public float detal_pitch;
        public float detal_dstToTarget;

        public void SetCameraDetal(V.LECameraManager.CameraDelta e)
        {
            detal_yaw = e.delta_yaw;
            detal_pitch = e.delta_pitch;
            detal_dstToTarget = e.delta_dstToTarget;
        }

        public float Yaw
        {
            get { return yaw; }
        }

        public float Pitch
        {
            get { return pitch; }
        }

        float maxOffset = 0.2f;
        Vector3 offsetPos;

        Vector3 Anti_Vibration_Pos()
        {
            if ((offsetPos - target.position).sqrMagnitude > maxOffset)
            {
                offsetPos = target.position;
            }
            return offsetPos - transform.forward * dstToTarget;
        }
    }
}
