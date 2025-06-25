using Controller2DProject.Controllers.Inputs;
using UnityEngine;
using UnityUtils.StateMachine;
namespace Controller2DProject.Controllers.States
{
    public class RunState : IState
    {
        private readonly PlayerControllerStates _playerController;
        private readonly InputReader _input;
        private readonly PlayerData _playerData;
        private readonly Rigidbody2D _rb;

        public RunState(PlayerControllerStates playerController, InputReader input, PlayerData playerData, Rigidbody2D rb)
        {
            _playerController = playerController;
            _input = input;
            _playerData = playerData;
            _rb = rb;
        }
        
        public void OnEnter()
        {
            _playerController.SetGravityScale(_playerData.GravityScale);
            _playerController.Animator.Play("Run");
        }

        public void FixedUpdate()
        {
            Move(1);
        }

        private void Move(float lerpAmount)
        {
            //Calculate the direction we want to move in and our desired velocity
            float targetSpeed = _input.Direction.x * _playerData.RunMaxSpeed;
            //We can reduce are control using Lerp() this smooths changes to are direction and speed
            targetSpeed = Mathf.Lerp(_rb.linearVelocity.x, targetSpeed, lerpAmount);
            
            //Gets an acceleration value based on if we are accelerating (includes turning) 
            //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _playerData.RunAccelAmount : _playerData.RunDeccelAmount;
            
            //Calculate difference between current velocity and desired velocity
            //Calculate force along x-axis to apply to thr player
            float speedDif = targetSpeed - _rb.linearVelocity.x;
            float movement = speedDif * accelRate;

            //Convert this to a vector and apply to rigidbody
            _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
    }
}