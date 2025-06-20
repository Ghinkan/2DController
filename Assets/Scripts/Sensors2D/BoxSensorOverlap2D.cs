using UnityEngine;
namespace Sensors2D
{
    [System.Serializable]
    public class BoxSensorOverlap2D : SensorOverlap2D
    {
        [SerializeField] private Vector2 _groundBoxCheckSize;
        
        public override void Cast()
        {
            DetectedCollider = Physics2D.OverlapBox(Transform.position, _groundBoxCheckSize, 0, LayerMask);
        }
        
        public override void DrawGizmos()
        {
            if (!Transform) return;

            Vector2 center = Transform.position;
            Vector2 halfSize = _groundBoxCheckSize * 0.5f;

            Vector3 topLeft = center + new Vector2(-halfSize.x, halfSize.y);
            Vector3 topRight = center + new Vector2(halfSize.x, halfSize.y);
            Vector3 bottomLeft = center + new Vector2(-halfSize.x, -halfSize.y);
            Vector3 bottomRight = center + new Vector2(halfSize.x, -halfSize.y);
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}