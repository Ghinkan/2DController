using Controller2DProject.Controllers.Inputs;
using DG.Tweening;
using UnityEngine;
namespace Controller2DProject.Controllers
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        [SerializeField] private float _rotationTime;

        private bool _isFacingRight;
        
        private const float DirectionThreshold = 0.01f;
        private const float RightRotation = 0f;
        private const float LeftRotation = 180f;
        
        private void Awake()
        {
            _isFacingRight = true;
        }
        
        private void OnEnable()
        {
            _input.Move += CheckDirectionToFace;
        }

        private void OnDisable()
        {
            _input.Move -= CheckDirectionToFace;
        }
        
        private void CheckDirectionToFace(Vector2 direction)
        {
            if (!ShouldUpdateCameraRotation(direction)) return;

            bool wantsToFaceRight = direction.x > 0;
            if (wantsToFaceRight != _isFacingRight)
                RotateCamera(wantsToFaceRight);
        }

        private void RotateCamera(bool faceRight)
        {
            transform.DORotate(CalculateTargetRotation(faceRight), _rotationTime).SetEase(Ease.InOutSine);
        }
        
        private Vector3 CalculateTargetRotation(bool faceRight)
        {
            _isFacingRight = faceRight;
            float yRotation = _isFacingRight ? RightRotation : LeftRotation;
            return new Vector3(0f, yRotation, 0f);
        }
        
        private bool ShouldUpdateCameraRotation(Vector2 direction)
        {
            return Mathf.Abs(direction.x) >= DirectionThreshold;
        }
    }
}