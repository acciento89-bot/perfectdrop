using UnityEngine;
using Kamilunavo.PerfectDrop.Input;

namespace Kamilunavo.PerfectDrop.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        public VirtualJoystick Joystick;
        public HoldButton JumpButton;
        public Transform CameraTransform;
        public PrecisionCourse Course;
        public float MoveSpeed = 7.2f;
        public float JumpSpeed = 8.8f;
        public float Gravity = 22f;
        public bool InputEnabled { get; set; } = true;
        private CharacterController _controller;
        private float _vertical;
        private float _boostMultiplier = 1f;
        private float _boostUntil;

        private void Awake() => _controller = GetComponent<CharacterController>();

        private void Update()
        {
            if (Time.time > _boostUntil) _boostMultiplier = 1f;
            if (!InputEnabled)
            {
                if (_controller.isGrounded) _vertical = -2f;
                else _vertical -= Gravity * Time.deltaTime;
                _controller.Move(Vector3.up * (_vertical * Time.deltaTime));
                return;
            }

            var touch = Joystick != null ? Joystick.Value : Vector2.zero;
            var keyboard = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
            var move = touch.sqrMagnitude > 0.01f ? touch : Vector2.ClampMagnitude(keyboard, 1f);
            var forward = CameraTransform != null ? CameraTransform.forward : Vector3.forward;
            var right = CameraTransform != null ? CameraTransform.right : Vector3.right;
            forward.y = 0f; right.y = 0f; forward.Normalize(); right.Normalize();
            var planar = forward * move.y + right * move.x;
            if (planar.sqrMagnitude > 0.001f) transform.forward = Vector3.Slerp(transform.forward, planar.normalized, 1f - Mathf.Exp(-14f * Time.deltaTime));
            if (_controller.isGrounded && _vertical < 0f) _vertical = -2f;
            var wantsJump = UnityEngine.Input.GetKeyDown(KeyCode.Space) || (JumpButton != null && JumpButton.ConsumePress());
            if (_controller.isGrounded && wantsJump) _vertical = JumpSpeed * _boostMultiplier;
            _vertical -= Gravity * Time.deltaTime;
            var velocity = planar * (MoveSpeed * _boostMultiplier);
            velocity.y = _vertical;
            _controller.Move(velocity * Time.deltaTime);
        }

        public void ApplyBoost(float multiplier, float duration)
        {
            if (!InputEnabled) return;
            _boostMultiplier = Mathf.Max(1f, multiplier);
            _boostUntil = Time.time + Mathf.Max(0.1f, duration);
        }

        public void ResetMotion()
        {
            _vertical = -2f;
            _boostMultiplier = 1f;
            _boostUntil = 0f;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.normal.y < 0.45f) return;
            var platform = hit.collider.GetComponent<PrecisionPlatform>();
            if (platform != null) Course?.RegisterLanding(platform, transform.position);
        }
    }
}
