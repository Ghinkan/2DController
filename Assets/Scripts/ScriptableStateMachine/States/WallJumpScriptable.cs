using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine
{
    [CreateAssetMenu(fileName = "WallJumpState", menuName = "StateMachine/States/WallJumpState", order = 0)]
    public class WallJumpScriptable : State
    {
        public PlayerContext PlayerContext;
        
        public override void OnEnter()
        {
            int dir = (PlayerContext.PlayerController.LastOnWallRightTime.IsRunning) ? -1 : 1;
            
            PlayerContext.PlayerController.LastPressedJumpTime.Stop();
            PlayerContext.PlayerController.LastOnGroundTimer.Stop();
            PlayerContext.PlayerController.LastOnWallRightTime.Stop();
            PlayerContext.PlayerController.LastOnWallLeftTime.Stop();
            PlayerContext.PlayerController.LastOnWallTimer.Stop();
            PlayerContext.PlayerController.IsJumpCut = false;
            WallJump(dir);
            
            PlayerContext.PlayerController.Animator.Play("Jump");
        }
        
        public override void Update()
        {
            if(!PlayerContext.PlayerController.Input.IsJumpKeyPressed())
                PlayerContext.PlayerController.IsJumpCut = true;
            
            if (PlayerContext.PlayerController.IsJumpCut)
                PlayerContext.PlayerController.SetGravityScale(PlayerContext.PlayerController.PlayerData.GravityScale * PlayerContext.PlayerController.PlayerData.JumpCutGravityMult);
            else if (Mathf.Abs(PlayerContext.PlayerController.Rb.linearVelocity.y) < PlayerContext.PlayerController.PlayerData.JumpHangTimeThreshold)
                PlayerContext.PlayerController.SetGravityScale(PlayerContext.PlayerController.PlayerData.GravityScale * PlayerContext.PlayerController.PlayerData.JumpHangGravityMult);
            else
                PlayerContext.PlayerController.SetGravityScale(PlayerContext.PlayerController.PlayerData.GravityScale);
        }

        public override void FixedUpdate()
        {
            Move(PlayerContext.PlayerController.PlayerData.WallJumpRunLerp);
        }
        
        private void WallJump(int dir)
        {
            Vector2 force = new Vector2(PlayerContext.PlayerController.PlayerData.WallJumpForce.x, PlayerContext.PlayerController.PlayerData.WallJumpForce.y);
            force.x *= dir; //apply force in opposite direction of wall

            if (Mathf.Sign(PlayerContext.PlayerController.Rb.linearVelocity.x) != Mathf.Sign(force.x))
                force.x -= PlayerContext.PlayerController.Rb.linearVelocity.x;
            
            //Checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            if (PlayerContext.PlayerController.Rb.linearVelocity.y < 0)
                force.y -= PlayerContext.PlayerController.Rb.linearVelocity.y;

            //Unlike in the run we want to use the Impulse mode.
            //The default mode will apply are force instantly ignoring masss
            PlayerContext.PlayerController.Rb.AddForce(force, ForceMode2D.Impulse);
        }
        
        private void Move(float lerpAmount)
        {
            //Calculate the direction we want to move in and our desired velocity
            float targetSpeed = PlayerContext.PlayerController.Input.Direction.x * PlayerContext.PlayerController.PlayerData.RunMaxSpeed;
            //We can reduce are control using Lerp() this smooths changes to are direction and speed
            targetSpeed = Mathf.Lerp(PlayerContext.PlayerController.Rb.linearVelocity.x, targetSpeed, lerpAmount);
            
            //Gets an acceleration value based on if we are accelerating (includes turning) 
            //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? PlayerContext.PlayerController.PlayerData.RunAccelAmount * PlayerContext.PlayerController.PlayerData.AccelInAir : PlayerContext.PlayerController.PlayerData.RunDeccelAmount * PlayerContext.PlayerController.PlayerData.DeccelInAir;

            //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
            if (Mathf.Abs(PlayerContext.PlayerController.Rb.linearVelocity.y) < PlayerContext.PlayerController.PlayerData.JumpHangTimeThreshold)
            {
                accelRate *= PlayerContext.PlayerController.PlayerData.JumpHangAccelerationMult;
                targetSpeed *= PlayerContext.PlayerController.PlayerData.JumpHangMaxSpeedMult;
            }
            
            //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
            if (PlayerContext.PlayerController.PlayerData.DoConserveMomentum && Mathf.Abs(PlayerContext.PlayerController.Rb.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(PlayerContext.PlayerController.Rb.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f)
            {
                //Prevent any deceleration from happening, or in other words conserve are current momentum
                //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
                accelRate = 0;
            }
            
            //Calculate difference between current velocity and desired velocity
            //Calculate force along x-axis to apply to the player
            float speedDif = targetSpeed - PlayerContext.PlayerController.Rb.linearVelocity.x;
            float movement = speedDif * accelRate;

            //Convert this to a vector and apply to rigidbody
            PlayerContext.PlayerController.Rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
    }
}