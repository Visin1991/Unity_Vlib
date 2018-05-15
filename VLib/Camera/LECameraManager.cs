using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this Script on the Camera
/// Put the Camera as a child of the Player root
/// </summary>
namespace V
{
    public class LECameraManager : MonoBehaviour
    {
        public bool Freeze_Yaw;
        public bool Freeze_Pitch;

        public CameraType cameraType = CameraType.FirstPerson;

        PlayerCamera currentCamera;
        public PlayerCamera CurrentCamera { get { return currentCamera; } }

        LECameraManager.CameraDelta cameraDelta;

        // Use this for initialization
        void Start()
        {
            if (cameraType == CameraType.FirstPerson)
            {
                currentCamera = GetComponentInChildren<FirstPersonCamera>();
            }
            else if (cameraType == CameraType.ThridPerson)
            {
                currentCamera = GetComponentInChildren<ThridPersonCamera>();
            }
            currentCamera.InitialCamera();
        }

        //Update is called once per frame
        public void UpdateCameraManager(UserInputData userInput)
        {
            //Input for Camera
            if (userInput.mouseScroll != 0 || userInput.mouseHorizontal != 0 || userInput.mouseVertical != 0)
            {
                cameraDelta.delta_pitch = userInput.mouseVertical;
                cameraDelta.delta_yaw = userInput.mouseHorizontal;
                cameraDelta.delta_dstToTarget = userInput.mouseScroll;

                SetCameraDelta(cameraDelta);

                userInput.mouseVertical = 0;
                userInput.mouseHorizontal = 0;
            }

            currentCamera.UpdateCamera();
        }

        private void LateUpdate()
        {
            currentCamera.LateUpdateCamera();
        }

        public void SetCameraDelta(CameraDelta delta)
        {
            if (Freeze_Yaw) { delta.delta_yaw = 0.0f; }
            if (Freeze_Pitch) { delta.delta_pitch = 0.0f; }
            currentCamera.SetCameraDetal(delta);
        }

        public float Yaw()
        {
            return currentCamera.Yaw;
        }

        public struct CameraDelta
        {
            public float delta_yaw;
            public float delta_pitch;
            public float delta_dstToTarget;

        }
    }
}
