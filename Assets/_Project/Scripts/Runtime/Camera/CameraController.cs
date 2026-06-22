using System;
using NJG.Utilities;
using Unity.Cinemachine;
using UnityEngine;

namespace NJG.Runtime.Cam
{
    public class CameraController : Singleton<CameraController>
    {
        [field: SerializeField]
        public CinemachineCamera FollowCamera { get; private set; }
        
        public Camera MainCamera { get; private set; }

        public override void Awake()
        {
            base.Awake();
            MainCamera = Camera.main ?? GetComponent<Camera>();
            if (FollowCamera == null)
            {
                Log.I("FollowCamera is not assigned, trying to find one in the scene.");
                FollowCamera = FindAnyObjectByType<CinemachineCamera>();
            }
        }

        public void SetFollowTarget(Transform target)
        {
            if (FollowCamera != null)
            {
                FollowCamera.Follow = target;
                FollowCamera.LookAt = target;
            }
            else
                Log.W("FollowCamera is not assigned, cannot set follow target.");
        }
    }
}