using UnityEngine;
namespace Sensors2D
{
    public interface ISensor
    {
        public bool       HasDetectedHit();
        public Collider2D GetCollider();
        public Transform  GetTransform();
        public Vector3    GetPosition();
        public Vector3    GetNormal();
        public float      GetDistance();
        
        public void       Cast();
        public void       DrawGizmos();
    }
}