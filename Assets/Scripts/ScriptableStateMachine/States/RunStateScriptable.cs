using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine
{
    [CreateAssetMenu(fileName = "RunState", menuName = "StateMachine/States/RunState", order = 0)]
    public class RunStateScriptable : State
    {
        public PlayerContext PlayerContext;
        
        public override void OnEnter()
        {
            PlayerContext.PlayerController.SetGravityScale(PlayerContext.PlayerController.PlayerData.GravityScale);
            PlayerContext.PlayerController.Animator.Play("Run");
        }

        public override void FixedUpdate()
        {
            Move(1);
        }

        private void Move(float lerpAmount)
        {
            float targetSpeed = PlayerContext.PlayerController.Input.Direction.x * PlayerContext.PlayerController.PlayerData.RunMaxSpeed;
            targetSpeed = Mathf.Lerp(PlayerContext.PlayerController.Rb.linearVelocity.x, targetSpeed, lerpAmount);
            
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? PlayerContext.PlayerController.PlayerData.RunAccelAmount : PlayerContext.PlayerController.PlayerData.RunDeccelAmount;
            float speedDif = targetSpeed - PlayerContext.PlayerController.Rb.linearVelocity.x;
            float movement = speedDif * accelRate;
            
            PlayerContext.PlayerController.Rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
    }
}