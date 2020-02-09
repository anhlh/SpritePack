using OriTool.GoPool;
using UnityEngine;

namespace OriTool.Example
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float maxVelocity = 10;
        [SerializeField] private float maxDistance = 5f;

        private Transform _transform;
        private Transform Transform => _transform != null ? _transform : _transform = transform;

        private Rigidbody2D _rb;
        private Rigidbody2D Rb => _rb != null ? _rb : _rb = GetComponent<Rigidbody2D>();

        private Vector3 _originPos;

        private bool _isReady;

        public void Emit()
        {
            _originPos = Transform.position;
            _isReady = true;
            Rb.velocity = Transform.up.normalized * maxVelocity;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_isReady) return;
            var distance = Vector3.Distance(_originPos, Transform.position);
            if (distance > maxDistance) PutToPool();
        }

        private void PutToPool()
        {
            _isReady = false;
            GOPoolManager.Put(this);
        }
    }
}