using Controller2DProject.Controllers.Inputs;
using UnityEngine;
using UnityUtils.StateMachine;
namespace Controller2DProject.Controllers.States
{
    public class WallJumpState : IState
    {
        private readonly PlayerControllerStates _playerController;
        private readonly InputReader _input;
        private readonly PlayerData _playerData;
        private readonly Rigidbody2D _rb;
        
        public WallJumpState(PlayerControllerStates playerController, InputReader input, PlayerData playerData, Rigidbody2D rb)
        {
            _playerController = playerController;
            _input = input;
            _playerData = playerData;
            _rb = rb;
        }
        
        public void OnEnter()
        {
            int dir = (_playerController.LastOnWallRightTime.IsRunning) ? -1 : 1;
            
            _playerController.LastPressedJumpTime.Stop();
            _playerController.LastOnGroundTimer.Stop();
            _playerController.LastOnWallRightTime.Stop();
            _playerController.LastOnWallLeftTime.Stop();
            
            _playerController.SetGravityScale(_playerData.GravityScale * _playerData.JumpHangGravityMult);
            WallJump(dir);
        }
        
        public void Update()
        {
            if(!_input.IsJumpKeyPressed())
                _playerController.IsJumpCut = true;
        }

        public void FixedUpdate()
        {
            if (_playerController.IsJumpCut)
            {
                //Higher gravity if jump button released
                _playerController.SetGravityScale(_playerData.GravityScale * _playerData.JumpCutGravityMult);
            }
            else if (Mathf.Abs(_rb.linearVelocity.y) < _playerData.JumpHangTimeThreshold)
            {
                _playerController.SetGravityScale(_playerData.GravityScale * _playerData.JumpHangGravityMult);
            }
            else
                _playerController.SetGravityScale(_playerData.GravityScale);
            
            Move(_playerData.WallJumpRunLerp);
        }
        
        public void OnExit()
        {
            _playerController.IsJumpCut = false;
        }

        private void WallJump(int dir)
        {
            Vector2 force = new Vector2(_playerData.WallJumpForce.x, _playerData.WallJumpForce.y);
            force.x *= dir; //apply force in opposite direction of wall

            if (Mathf.Sign(_rb.linearVelocity.x) != Mathf.Sign(force.x))
                force.x -= _rb.linearVelocity.x;
            
            //Checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            if (_rb.linearVelocity.y < 0)
                force.y -= _rb.linearVelocity.y;

            //Unlike in the run we want to use the Impulse mode.
            //The default mode will apply are force instantly ignoring masss
            _rb.AddForce(force, ForceMode2D.Impulse);
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