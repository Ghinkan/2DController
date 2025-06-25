using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
namespace Controller2DProject.Controllers.Cameras
{
    [RequireComponent(typeof(CameraController))]
    public class CameraDamping : MonoBehaviour
    {
        [SerializeField] private float _fallSpeedYDampingThreshold;
        [SerializeField] private float _fallDamping;
        [SerializeField] private float _transitionTime;
        
        private CinemachinePositionComposer _positionComposer;
        private float _normalDamping;
        
        private void Start()
        {
            _positionComposer = CameraManager.Instance.CurrentCamera.GetComponent<CinemachinePositionComposer>();
            _normalDamping = _positionComposer.Damping.y;
        }

        private bool _isTransitioning;
        private bool _isFalling;

        public void UpdateDamping(float verticalVelocity)
        {
            if (ShouldStartFalling(verticalVelocity))
                TransitionToFallDamping();
            else if (ShouldReturnToNormal(verticalVelocity))
                TransitionToNormalDamping();
        }

        private bool ShouldStartFalling(float verticalVelocity) =>
            verticalVelocity < _fallSpeedYDampingThreshold && !_isTransitioning && !_isFalling;

        private bool ShouldReturnToNormal(float verticalVelocity) =>
            verticalVelocity >= 0f && !_isTransitioning && _isFalling;

        private void TransitionToFallDamping()
        {
            TransitionDamping(_fallDamping, true);
        }

        private void TransitionToNormalDamping()
        {
            TransitionDamping(_normalDamping, false);
        }

        private void TransitionDamping(float targetDamping, bool isFalling)
        {
            _isTransitioning = true;
            _isFalling = isFalling;
        
            DOTween.To(
                () => _positionComposer.Damping.y,
                y => _positionComposer.Damping.y = y,
                targetDamping,
                _transitionTime
            ).OnComplete(() => _isTransitioning = false);
        }
    }
}