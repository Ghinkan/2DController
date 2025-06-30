using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine
{
    [CreateAssetMenu(fileName = "WallSlideState", menuName = "StateMachine/States/WallSlideState", order = 0)]
    public class WallSlideScriptable : State
    {
        public PlayerContext PlayerContext;

        public override void OnEnter()
        {
            PlayerContext.PlayerController.SetGravityScale(0);
            PlayerContext.PlayerController.Animator.Play("WallSlide");
        }

        public override void FixedUpdate()
        {
            Slide();
        }

        private void Slide()
        {
            //Works the same as the Run, but only in the y-axis
            //THis seems to work fine, but maybe you'll find a better way to implement a slide into this system
            float speedDif = PlayerContext.PlayerController.PlayerData.SlideSpeed - PlayerContext.PlayerController.Rb.linearVelocity.y;	
            float movement = speedDif * PlayerContext.PlayerController.PlayerData.SlideAccel;
            
            //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
            //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
            movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

            PlayerContext.PlayerController.Rb.AddForce(movement * Vector2.up);
        }
    }
}