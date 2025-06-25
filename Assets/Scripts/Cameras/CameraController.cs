using Controller2DProject.Controllers.Inputs;
using UnityEngine;
namespace Controller2DProject.Controllers.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        [SerializeField] private CameraHorizontalFollow _cameraHorizontalFollow;
        [SerializeField] private CameraDamping _cameraDamping;

        private Transform _tr;
        
        private bool _isFacingRight;
        private float _lastYPosition;
        
        private void Awake()
        {
            _tr = transform;
        }
        
        private void OnEnable()
        {
            _input.Move += _cameraHorizontalFollow.CheckDirectionToFace;
        }

        private void OnDisable()
        {
            _input.Move -= _cameraHorizontalFollow.CheckDirectionToFace;
        }

        private void Update()
        {
            CheckFallingVelocity();
        }

        private void CheckFallingVelocity()
        {
            float currentYPosition = _tr.position.y;
            float yDelta = (currentYPosition - _lastYPosition) / Time.deltaTime;
            _cameraDamping.UpdateDamping(yDelta);
            _lastYPosition = currentYPosition;
        }
    }
}