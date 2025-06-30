using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine
{
    [CreateAssetMenu(fileName = "FallState", menuName = "StateMachine/States/FallState", order = 0)]
    public class FallStateScriptable : State
    {
        public PlayerContext PlayerContext;
        
        public override void OnEnter()
        {
            PlayerContext.PlayerController.Animator.Play("Fall");
        }

        public override void Update()
        {
            if (PlayerContext.PlayerController.Rb.linearVelocity.y < 0 && PlayerContext.PlayerController.Input.Direction.y < 0)
            {
                //Much higher gravity if holding down
                PlayerContext.PlayerController.SetGravityScale(PlayerContext.PlayerController.PlayerData.GravityScale * PlayerContext.PlayerController.PlayerData.FastFallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                PlayerContext.PlayerController.Rb.linearVelocity = new Vector2(PlayerContext.PlayerController.Rb.linearVelocity.x, Mathf.Max(PlayerContext.PlayerController.Rb.linearVelocity.y, -PlayerContext.PlayerController.PlayerData.MaxFastFallSpeed));
            }
            else if (PlayerContext.PlayerController.IsJumpCut)
            {
                //Higher gravity if jump button released
                PlayerContext.PlayerController.SetGravityScale(PlayerContext.PlayerController.PlayerData.GravityScale * PlayerContext.PlayerController.PlayerData.JumpCutGravityMult);
                PlayerContext.PlayerController.Rb.linearVelocity = new Vector2(PlayerContext.PlayerController.Rb.linearVelocity.x, Mathf.Max(PlayerContext.PlayerController.Rb.linearVelocity.y, -PlayerContext.PlayerController.PlayerData.MaxFallSpeed));
            }
            else
            {
                //Higher gravity if falling
                PlayerContext.PlayerController.SetGravityScale(PlayerContext.PlayerController.PlayerData.GravityScale * PlayerContext.PlayerController.PlayerData.FallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                PlayerContext.PlayerController.Rb.linearVelocity = new Vector2(PlayerContext.PlayerController.Rb.linearVelocity.x, Mathf.Max(PlayerContext.PlayerController.Rb.linearVelocity.y, -PlayerContext.PlayerController.PlayerData.MaxFallSpeed));
            }
        }

        public override void FixedUpdate()
        {
            Move(1);
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
            
            //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
            if (PlayerContext.PlayerController.PlayerData.DoConserveMomentum && Mathf.Abs(PlayerContext.PlayerController.Rb.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(PlayerContext.PlayerController.Rb.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f)
            {
                //Prevent any deceleration from happening, or in other words conserve are current momentum
                //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
                accelRate = 0;
            }
            
            //Calculate difference between current velocity and desired velocity
            //Calculate force along x-axis to apply to thr player
            float speedDif = targetSpeed - PlayerContext.PlayerController.Rb.linearVelocity.x;
            float movement = speedDif * accelRate;

            //Convert this to a vector and apply to rigidbody
            PlayerContext.PlayerController.Rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
    }
}