using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
namespace Controller2DProject.Controllers.Cameras
{
    public class CameraHorizontalFollow : MonoBehaviour
    {
        [SerializeField] private float _rotationTime = 0.5f;
        
        private const float DirectionThreshold = 0.01f;
        
        private CinemachinePositionComposer _positionComposer;
        private float _offsetXAmount;
        private bool _isFacingRight = true;
        
        private void Start()
        {
            _positionComposer = CameraManager.Instance.CurrentCamera.GetComponent<CinemachinePositionComposer>();
            _offsetXAmount = _positionComposer.TargetOffset.x;
        }
        
        public void CheckDirectionToFace(Vector2 direction)
        {
            if (!ShouldUpdateCameraRotation(direction)) return;

            bool wantsToFaceRight = direction.x > 0;
            if (wantsToFaceRight != _isFacingRight)
                AdjustCameraOffset(wantsToFaceRight);
        }
        
        private bool ShouldUpdateCameraRotation(Vector2 direction)
        {
            return Mathf.Abs(direction.x) >= DirectionThreshold;
        }
        
        private void AdjustCameraOffset(bool faceRight)
        {
            _isFacingRight = faceRight;
            float targetOffsetX = _isFacingRight ? _offsetXAmount : -_offsetXAmount;
            
            DOTween.Kill(_positionComposer, complete: false);
            DOTween.To(
                () => _positionComposer.TargetOffset.x,
                x => {
                    Vector3 offset = _positionComposer.TargetOffset;
                    offset.x = x;
                    _positionComposer.TargetOffset = offset;
                },
                targetOffsetX,
                _rotationTime
            ).SetTarget(_positionComposer).SetEase(Ease.InOutSine);
        }
    }
}