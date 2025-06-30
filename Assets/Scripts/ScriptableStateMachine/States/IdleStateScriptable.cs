using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine
{
    [CreateAssetMenu(fileName = "IdleState", menuName = "StateMachine/States/IdleState", order = 0)]
    public class IdleStateScriptable : State
    {
        public PlayerContext PlayerContext;

        public override void OnEnter()
        {
            PlayerContext.PlayerController.Rb.linearVelocityX = 0f;
            PlayerContext.PlayerController.SetGravityScale(PlayerContext.PlayerController.PlayerData.GravityScale);
            
            PlayerContext.PlayerController.Animator.Play("Idle");
        }

        public override void Update()
        {
            PlayerContext.PlayerController.Rb.linearVelocityX = 0f;
        }
    }

}