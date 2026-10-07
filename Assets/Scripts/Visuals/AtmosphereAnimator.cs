using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    public sealed class AtmosphereAnimator : MonoBehaviour
    {
        public float CloudDrift = 0.20f;
        public float SunPulse = 0.035f;

        private Transform[] _clouds;
        private Transform _sun;
        private Vector3 _sunBaseScale;
        private Vector3[] _cloudOrigins;
        private float[] _cloudPhase;

        public void Bind(Transform atmosphere)
        {
            if (atmosphere == null) return;

            var clouds = new System.Collections.Generic.List<Transform>();
            for (var i = 0; i < atmosphere.childCount; i++)
            {
                var child = atmosphere.GetChild(i);
                if (child.name.StartsWith("Cloud_")) clouds.Add(child);
                if (child.name == "HorizonSun") _sun = child;
            }

            _clouds = clouds.ToArray();
            _cloudOrigins = new Vector3[_clouds.Length];
            _cloudPhase = new float[_clouds.Length];

            for (var i = 0; i < _clouds.Length; i++)
            {
                _cloudOrigins[i] = _clouds[i].localPosition;
                _cloudPhase[i] = i * 0.73f;
            }

            if (_sun != null) _sunBaseScale = _sun.localScale;
        }

        private void Update()
        {
            if (GamePreferences.ReducedMotion) return;

            var time = Time.time;
            if (_clouds != null)
            {
                for (var i = 0; i < _clouds.Length; i++)
                {
                    if (_clouds[i] == null) continue;
                    var origin = _cloudOrigins[i];
                    var offset = Mathf.Sin(time * 0.10f + _cloudPhase[i]) * CloudDrift;
                    _clouds[i].localPosition = origin + new Vector3(offset, 0f, 0f);
                }
            }

            if (_sun != null)
            {
                var pulse = 1f + Mathf.Sin(time * 0.75f) * SunPulse;
                _sun.localScale = _sunBaseScale * pulse;
            }
        }
    }

    public sealed class GoalBeaconAnimator : MonoBehaviour
    {
        public float RotationSpeed = 8f;
        private Vector3 _baseScale;

        private void Awake()
        {
            _baseScale = transform.localScale;
        }

        private void Update()
        {
            if (GamePreferences.ReducedMotion) return;
            transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime, Space.Self);
            var pulse = 1f + Mathf.Sin(Time.time * 1.35f) * 0.035f;
            transform.localScale = _baseScale * pulse;
        }
    }
}
