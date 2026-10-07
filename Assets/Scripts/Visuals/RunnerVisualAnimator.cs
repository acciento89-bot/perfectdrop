using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class RunnerVisualAnimator : MonoBehaviour
    {
        public Transform VisualRoot;
        public Transform LeftArm;
        public Transform RightArm;
        public Transform LeftLeg;
        public Transform RightLeg;

        private CharacterController _controller;
        private Vector3 _rootStart;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (VisualRoot != null) _rootStart = VisualRoot.localPosition;
        }

        private void LateUpdate()
        {
            if (_controller == null || VisualRoot == null) return;

            var horizontal = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;
            var move01 = Mathf.Clamp01(horizontal / 7.2f);
            var airborne = !_controller.isGrounded;
            var reduced = GamePreferences.ReducedMotion;
            var phase = Time.time * Mathf.Lerp(5f, 10.5f, move01);
            var stride = reduced ? 0f : Mathf.Sin(phase) * 26f * move01;

            if (LeftArm != null) LeftArm.localRotation = Quaternion.Euler(stride, 0f, 0f);
            if (RightArm != null) RightArm.localRotation = Quaternion.Euler(-stride, 0f, 0f);
            if (LeftLeg != null) LeftLeg.localRotation = Quaternion.Euler(-stride * 0.72f, 0f, 0f);
            if (RightLeg != null) RightLeg.localRotation = Quaternion.Euler(stride * 0.72f, 0f, 0f);

            var bob = reduced || airborne ? 0f : Mathf.Abs(Mathf.Sin(phase * 2f)) * 0.035f * move01;
            VisualRoot.localPosition = _rootStart + Vector3.up * bob;
            VisualRoot.localRotation = Quaternion.Euler(reduced ? 0f : (airborne ? -6f : 0f), 0f, 0f);
        }
    }
}
