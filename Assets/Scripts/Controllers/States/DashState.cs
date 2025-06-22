using System;
using System.Threading.Tasks;
using Controller2DProject.Controllers.Inputs;
using UnityEngine;
using UnityUtils.StateMachine;
namespace Controller2DProject.Controllers.States
{
    public class DashState: IState
    {
        private readonly PlayerControllerStates _playerController;
        private readonly InputReader _input;
        private readonly PlayerData _playerData;
        private readonly Rigidbody2D _rb;
        private readonly DashRefillManager _dashRefill;
        
        public bool IsDashing;
        
        private Vector2 _dashDirection;
        private bool _isDashAttacking;
        private float _dashTimer;
        private bool _isRefilling;
        private int _refillQueue;

        public int DashesLeft => _dashRefill.DashesLeft;
        
        public DashState(PlayerControllerStates playerController, InputReader input, PlayerData playerData, Rigidbody2D rb)
        {
            _playerController = playerController;
            _input = input;
            _playerData = playerData;
            _rb = rb;
            
            _dashRefill = new DashRefillManager(playerData);
        }

        public void OnEnter()
        {
            //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
            _ = SleepAsync(_playerData.DashSleepTime);

            _playerController.LastPressedDashTime.Stop();
            _playerController.LastOnGroundTimer.Stop();
            _playerController.SetGravityScale(0);
            
            IsDashing = true;
            _isDashAttacking = true;
            _dashTimer = _playerData.DashAttackTime;
            _dashDirection = GetDashDirection();
            
            _dashRefill.ConsumeDash();
        }
        
        public void Update()
        {
            if(!IsDashing) return;
            
            if (_isDashAttacking)
            {
                _rb.linearVelocity = _dashDirection.normalized * _playerData.DashSpeed;

                if (_dashTimer <= 0f)
                {
                    _isDashAttacking = false;
                    _dashTimer = _playerData.DashEndTime;
                    _playerController.SetGravityScale(_playerData.GravityScale);
                    _rb.linearVelocity = _dashDirection.normalized * _playerData.DashEndSpeed;
                }
            }
            else
            {
                Move(_playerData.DashEndRunLerp);
                
                if (_dashTimer <= 0f)
                    IsDashing = false;
            }
            
            _dashTimer -= Time.deltaTime;
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
        
        private async Task SleepAsync(float duration)
        {
            Time.timeScale = 0;
            await Task.Delay(TimeSpan.FromSeconds(duration));
            Time.timeScale = 1;
        }
        
        private Vector2 GetDashDirection()
        {
            return _input.Direction != Vector2.zero
                ? _input.Direction
                : (_playerController.IsFacingRight ? Vector2.right : Vector2.left);
        }
    }
}