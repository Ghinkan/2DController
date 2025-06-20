using Controller2DProject.Controllers.Inputs;
using UnityEngine;
using UnityUtils.StateMachine;
namespace Controller2DProject.Controllers.States
{
    public class JumpState : IState
    {
        private readonly InputReader _input;
        private readonly PlayerData _playerData;
        private readonly Rigidbody2D _rb;
        
        public JumpState(InputReader input, PlayerData playerData, Rigidbody2D rb)
        {
            _input = input;
            _playerData = playerData;
            _rb = rb;
        }

        public void OnEnter()
        {
            Jump();
        }

        public void FixedUpdate()
        {
            Move(1);
        }

        private void Jump()
        {
            // IsJumping = true;
            // IsWallJumping = false;
            // _isJumpCut = false;
            // _isJumpFalling = false;
            
            //Ensures we can't call Jump multiple times from one press
            // _lastPressedJumpTime.Stop();
            // _lastOnGroundTimer.Stop();
            
            //We increase the force applied if we are falling
            //This means we'll always feel like we jump the same amount 
            //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
            float force = _playerData.JumpForce;
            if (_rb.linearVelocity.y < 0)
                force -= _rb.linearVelocity.y;

            _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }
        
        private void Move(float lerpAmount)
        {
            //Calculate the direction we want to move in and our desired velocity
            float targetSpeed = _input.Direction.x * _playerData.RunMaxSpeed;
            //We can reduce are control using Lerp() this smooths changes to are direction and speed
            targetSpeed = Mathf.Lerp(_rb.linearVelocity.x, targetSpeed, lerpAmount);
            
            //Gets an acceleration value based on if we are accelerating (includes turning) 
            //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _playerData.RunAccelAmount * _playerData.AccelInAir : _playerData.RunDeccelAmount * _playerData.DeccelInAir;

            //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
            if (Mathf.Abs(_rb.linearVelocity.y) < _playerData.JumpHangTimeThreshold)
            {
                accelRate *= _playerData.JumpHangAccelerationMult;
                targetSpeed *= _playerData.JumpHangMaxSpeedMult;
            }
            
            //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
            if (_playerData.DoConserveMomentum && Mathf.Abs(_rb.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(_rb.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f)
            {
                //Prevent any deceleration from happening, or in other words conserve are current momentum
                //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
                accelRate = 0;
            }
            
            //Calculate difference between current velocity and desired velocity
            //Calculate force along x-axis to apply to the player
            float speedDif = targetSpeed - _rb.linearVelocity.x;
            float movement = speedDif * accelRate;

            //Convert this to a vector and apply to rigidbody
            _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
    }
}