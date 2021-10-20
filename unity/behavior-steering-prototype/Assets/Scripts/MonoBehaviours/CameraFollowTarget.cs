using System;
using UnityEngine;

namespace MonoBehaviours
{
    public interface IFollowTargets<T>
    {
        public T Target { get; }
        public void Follow();
    }
    public class CameraFollowTarget : MonoBehaviour, IFollowTargets<GameObject>
    {
        public Camera Camera;
        public GameObject FollowTarget;
        private Camera _camera;
        private Vector3 _offset;
        public GameObject Target { get; private set; }

        public void Follow()
        {
            if (FollowTarget != null)
            {
                _camera.transform.position = FollowTarget.transform.position + _offset;
            }
        }

        private void Start()
        {
            _camera = Camera != null ? Camera : Camera.current;

            if (FollowTarget == null)
                return;
            Target = FollowTarget;
            _offset = _camera.transform.position - Target.transform.position;
        }

        private void Update()
        {
            Follow();
        }
    }
}