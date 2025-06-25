using Controller2DProject.Controllers.Inputs;
using UnityEngine;
using UnityUtils.StateMachine;
namespace Controller2DProject.Controllers.States
{
    public class WallSlide : IState
    {
        private readonly PlayerControllerStates _playerController;
        private readonly InputReader _input;
        private readonly PlayerData _playerData;
        private readonly Rigidbody2D _rb;

        public WallSlide(PlayerControllerStates playerController, InputReader input, PlayerData playerData, Rigidbody2D rb)
        {
            _playerController = playerController;
            _input = input;
            _playerData = playerData;
            _rb = rb;
        }

        public void OnEnter()
        {
            _playerController.SetGravityScale(0);
            _playerController.Animator.Play("WallSlide");
        }

        public void FixedUpdate()
        {
            Slide();
        }

        private void Slide()
        {
            //Works the same as the Run, but only in the y-axis
            //THis seems to work fine, but maybe you'll find a better way to implement a slide into this system
            float speedDif = _playerData.SlideSpeed - _rb.linearVelocity.y;	
            float movement = speedDif * _playerData.SlideAccel;
            
            //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
            //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
            movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

            _rb.AddForce(movement * Vector2.up);
        }
    }
}