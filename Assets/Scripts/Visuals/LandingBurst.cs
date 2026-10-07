using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    public sealed class LandingBurst : MonoBehaviour
    {
        private Transform[] _pieces;
        private Vector3[] _velocity;
        private Vector3[] _scale;
        private float _age;
        private float _lifetime = 0.42f;

        public void Initialize(Transform[] pieces, Vector3[] velocity, float lifetime)
        {
            _pieces = pieces;
            _velocity = velocity;
            _scale=new Vector3[pieces.Length];
            for(var i=0;i<pieces.Length;i++)_scale[i]=pieces[i].localScale;
            _lifetime = Mathf.Max(0.1f, lifetime);
        }

        private void Update()
        {
            if (_pieces == null || _velocity == null)
            {
                Destroy(gameObject);
                return;
            }

            _age += Time.deltaTime;
            var t = Mathf.Clamp01(_age / _lifetime);
            var dt = Time.deltaTime;

            for (var i = 0; i < _pieces.Length; i++)
            {
                var piece = _pieces[i];
                if (piece == null) continue;

                _velocity[i] += Vector3.down * (4.8f * dt);
                piece.localPosition += _velocity[i] * dt;
                piece.localRotation *= Quaternion.Euler(0f, 160f * dt, 240f * dt);
                var scale = Mathf.Lerp(1f, 0.08f, t * t);
                piece.localScale = _scale[i] * scale;
            }

            if (_age >= _lifetime)
                Destroy(gameObject);
        }
    }
}
