using System;
using System.Threading.Tasks;
using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine
{
    [CreateAssetMenu(fileName = "DashState", menuName = "StateMachine/States/DashState", order = 0)]
    public class DashScriptable : State
    {
        public PlayerContext PlayerContext;
        
        private Vector2 _dashDirection;
        private bool _isDashAttacking;
        private float _dashTimer;
        
        public override void OnEnter()
        {
            //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
            _ = SleepAsync(PlayerContext.PlayerController.PlayerData.DashSleepTime);

            PlayerContext.PlayerController.LastPressedDashTime.Stop();
            PlayerContext.PlayerController.LastOnGroundTimer.Stop();
            PlayerContext.PlayerController.SetGravityScale(0);
            
            PlayerContext.IsDashing = true;
            _isDashAttacking = true;
            _dashTimer = PlayerContext.PlayerController.PlayerData.DashAttackTime;
            _dashDirection = GetDashDirection();
            
            PlayerContext.DashRefill.ConsumeDash();
            PlayerContext.PlayerController.Animator.Play("Dash");
        }
        
        public override void FixedUpdate()
        {
            if(!PlayerContext.IsDashing) return;
            
            _dashTimer -= Time.deltaTime;
            
            if (_isDashAttacking)
            {
                PlayerContext.PlayerController.Rb.linearVelocity = _dashDirection.normalized * PlayerContext.PlayerController.PlayerData.DashSpeed;

                if (_dashTimer <= 0f)
                {
                    _isDashAttacking = false;
                    _dashTimer = PlayerContext.PlayerController.PlayerData.DashEndTime;
                    PlayerContext.PlayerController.SetGravityScale(PlayerContext.PlayerController.PlayerData.GravityScale);
                    PlayerContext.PlayerController.Rb.linearVelocity = _dashDirection.normalized * PlayerContext.PlayerController.PlayerData.DashEndSpeed;
                }
            }
            else
            {
                Move(PlayerContext.PlayerController.PlayerData.DashEndRunLerp);
                
                if (_dashTimer <= 0f)
                    PlayerContext.IsDashing = false;
            }
        }
        
        private void Move(float lerpAmount)
        {
            //Calculate the direction we want to move in and our desired velocity
            float targetSpeed = PlayerContext.PlayerController.Input.Direction.x * PlayerContext.PlayerController.PlayerData.RunMaxSpeed;
            //We can reduce are control using Lerp() this smooths changes to are direction and speed
            targetSpeed = Mathf.Lerp(PlayerContext.PlayerController.Rb.linearVelocity.x, targetSpeed, lerpAmount);
            
            //Gets an acceleration value based on if we are accelerating (includes turning) 
            //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? PlayerContext.PlayerController.PlayerData.RunAccelAmount : PlayerContext.PlayerController.PlayerData.RunDeccelAmount;
            
            //Calculate difference between current velocity and desired velocity
            //Calculate force along x-axis to apply to thr player
            float speedDif = targetSpeed - PlayerContext.PlayerController.Rb.linearVelocity.x;
            float movement = speedDif * accelRate;

            //Convert this to a vector and apply to rigidbody
            PlayerContext.PlayerController.Rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
        
        private async Task SleepAsync(float duration)
        {
            Time.timeScale = 0;
            await Task.Delay(TimeSpan.FromSeconds(duration));
            Time.timeScale = 1;
        }
        
        private Vector2 GetDashDirection()
        {
            return PlayerContext.PlayerController.Input.Direction != Vector2.zero
                ? PlayerContext.PlayerController.Input.Direction
                : (PlayerContext.PlayerController.IsFacingRight ? Vector2.right : Vector2.left);
        }
    }
}