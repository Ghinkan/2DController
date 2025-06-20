using UnityEngine;
namespace Sensors2D
{
    [System.Serializable]
    public abstract class SensorCast2D : ISensor
    {
        [SerializeField] protected Transform Transform;
        [SerializeField] protected LayerMask LayerMask;
        [SerializeField] protected CastDirection CastDirection;
        [SerializeField] protected float CastLength;

        protected RaycastHit2D HitInfo;

        public         bool       HasDetectedHit()                          => HitInfo.collider;
        public virtual float      GetDistance()                             => HasDetectedHit() ? HitInfo.distance : 0f;
        public virtual Vector3    GetNormal()                               => HasDetectedHit() ? HitInfo.normal : Vector3.zero;
        public         Vector3    GetPosition()                             => HitInfo.point;
        public         Collider2D GetCollider()                             => HitInfo.collider;
        public         Transform  GetTransform()                            => HitInfo.transform;
        
        public         void       SetCastDirection(CastDirection direction) => CastDirection = direction;

        protected Vector3 GetCastDirection()
        {
            return CastDirection switch {
                CastDirection.Forward => Transform.forward,
                CastDirection.Right => Transform.right,
                CastDirection.Up => Transform.up,
                CastDirection.Backward => -Transform.forward,
                CastDirection.Left => -Transform.right,
                CastDirection.Down => -Transform.up,
                _ => Vector3.one,
            };
        }

        public abstract void Cast();
        public abstract void DrawGizmos();
    }
}