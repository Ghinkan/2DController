using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
namespace Controller2DProject.Controllers.Cameras
{
    [RequireComponent(typeof(CinemachinePositionComposer))]
    public class CameraDamping : MonoBehaviour
    {
        [SerializeField] private float _fallSpeedYDampingThreshold;
        [SerializeField] private float _fallDamping;
        [SerializeField] private float _transitionTime;
        
        private CinemachinePositionComposer _positionComposer;
        private Transform _playerTransform;
        private float _normalDamping;
        private float _lastYVelocity;
        private bool _isTransitioning;
        private bool _isFalling;

        public void Awake()
        {
            _positionComposer = GetComponent<CinemachinePositionComposer>();
            _normalDamping = _positionComposer.Damping.y;
            _playerTransform = _positionComposer.FollowTarget.transform;
        }
        
        public void Update()
        {
            float currentYPosition = _playerTransform.position.y;
            float yDelta = (currentYPosition - _lastYVelocity) / Time.deltaTime;
            _lastYVelocity = currentYPosition;
            
            UpdateDamping(yDelta);
        }
        
        private void UpdateDamping(float verticalVelocity)
        {        
            if (ShouldStartFalling(verticalVelocity))
                TransitionToFallDamping();
            else if (ShouldReturnToNormal(verticalVelocity))
                TransitionToNormalDamping();
        }

        private bool ShouldStartFalling(float verticalVelocity) => verticalVelocity < _fallSpeedYDampingThreshold && !_isTransitioning && !_isFalling;
        private bool ShouldReturnToNormal(float verticalVelocity) => verticalVelocity >= 0f && !_isTransitioning && _isFalling;

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