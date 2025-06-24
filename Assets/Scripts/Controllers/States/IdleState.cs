using Controller2DProject.Controllers.Inputs;
using UnityEngine;
using UnityUtils.StateMachine;
namespace Controller2DProject.Controllers.States
{
    public class IdleState : IState
    {
        private readonly PlayerControllerStates _playerController;
        private readonly InputReader _input;
        private readonly PlayerData _playerData;
        private readonly Rigidbody2D _rb;
        
        public IdleState(PlayerControllerStates playerController, InputReader input, PlayerData playerData, Rigidbody2D rb)
        {
            _playerController = playerController;
            _input = input;
            _playerData = playerData;
            _rb = rb;
        }

        public void OnEnter()
        {
            _rb.linearVelocityX = 0f;
            _playerController.SetGravityScale(_playerData.GravityScale);
        }

        public void Update()
        {
            _rb.linearVelocityX = 0f;
        }
    }
}