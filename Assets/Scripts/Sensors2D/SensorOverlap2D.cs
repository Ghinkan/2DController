using UnityEngine;
namespace Sensors2D
{
    [System.Serializable]
    public abstract class SensorOverlap2D : ISensor
    {
        [SerializeField] protected Transform Transform;
        [SerializeField] protected LayerMask LayerMask;

        protected Collider2D DetectedCollider;
        private Vector2 _contactPoint;

        public bool HasDetectedHit() => DetectedCollider;
        public float GetDistance()
        {
            if (!HasDetectedHit()) return 0f;

            float dist = Vector2.Distance(Transform.position, GetPosition());
            return dist > Mathf.Epsilon ? dist : 0f;
        }

        public Vector3 GetNormal()
        {
            if (!HasDetectedHit()) return Vector2.zero;

            Vector2 direction = Transform.position - GetPosition();
            return direction.sqrMagnitude < Mathf.Epsilon ? Vector2.up : direction.normalized;
        }

        public Vector3    GetPosition()  => HasDetectedHit() ? DetectedCollider.ClosestPoint(Transform.position) : Transform.position;
        public Collider2D GetCollider()  => DetectedCollider;
        public Transform  GetTransform() => HasDetectedHit() ? DetectedCollider.transform : null;

        public abstract void Cast();
        public abstract void DrawGizmos();
    }
}