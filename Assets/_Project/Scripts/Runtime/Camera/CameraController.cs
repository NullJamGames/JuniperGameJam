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
        [SerializeField]
        private Transform _distanceBlocker;
        [SerializeField]
        private float _blockerZOffset = 150f;
        
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

        private void Update()
        {
            _distanceBlocker.position = _distanceBlocker.position.WithZ(transform.position.z + _blockerZOffset);
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