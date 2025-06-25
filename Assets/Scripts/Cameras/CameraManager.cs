using UnityEngine;
using Unity.Cinemachine;
namespace Controller2DProject.Controllers.Cameras
{
    public class CameraManager : Singleton<CameraManager>
    {
        [SerializeField] private CinemachineCamera[] _cameras;
        public CinemachineCamera CurrentCamera { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();

            foreach (CinemachineCamera cinemachineCamera in _cameras)
            {
                if (cinemachineCamera.enabled)
                {
                    CurrentCamera = cinemachineCamera;
                    break;
                }
            }
        }
    }
}