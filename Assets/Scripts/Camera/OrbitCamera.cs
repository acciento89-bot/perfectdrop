using UnityEngine;
using UnityEngine.EventSystems;

namespace Kamilunavo.PerfectDrop.CameraSystem
{
    public sealed class OrbitCamera : MonoBehaviour
    {
        public Transform Target;
        public float Distance = 7.5f;
        public float Height = 3.2f;
        public float Sensitivity = 0.16f;
        private float _yaw;
        private float _pitch = 13f;
        private Vector2 _lastTouch;
        private int _orbitFinger = -1;

        private void LateUpdate()
        {
            if (Target == null) return;
            ReadOrbitInput();
            var rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            var focus = Target.position + Vector3.up * Height;
            var desired = focus - rotation * Vector3.forward * Distance;
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-12f * Time.deltaTime));
            transform.rotation = Quaternion.LookRotation(focus - transform.position, Vector3.up);
        }

        private void ReadOrbitInput()
        {
            if (UnityEngine.Input.GetMouseButton(1))
            {
                _yaw += UnityEngine.Input.GetAxis("Mouse X") * 3f;
                _pitch -= UnityEngine.Input.GetAxis("Mouse Y") * 2f;
            }
            if (UnityEngine.Input.touchCount == 0) { _orbitFinger = -1; return; }
            for (var i = 0; i < UnityEngine.Input.touchCount; i++)
            {
                var touch = UnityEngine.Input.GetTouch(i);
                if (_orbitFinger == -1 && touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    _orbitFinger = touch.fingerId;
                    _lastTouch = touch.position;
                }
                if (touch.fingerId != _orbitFinger) continue;
                if (touch.phase == TouchPhase.Moved)
                {
                    var delta = touch.position - _lastTouch;
                    _yaw += delta.x * Sensitivity;
                    _pitch -= delta.y * Sensitivity;
                    _lastTouch = touch.position;
                }
                if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled) _orbitFinger = -1;
            }
            _pitch = Mathf.Clamp(_pitch, -10f, 42f);
        }
    }
}
