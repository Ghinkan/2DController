using UnityEngine;
namespace Controller2DProject.Controllers
{
	[CreateAssetMenu(fileName = "PlayerData", menuName = "Controller2D/PlayerData", order = 0)]
	public class PlayerData : ScriptableObject
	{
		[Header("Gravity")]
		[HideInInspector] public float GravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
		[HideInInspector] public float GravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
		//Also the value the player's rigidbody2D.gravityScale is set to.
		[Space(5)]
		public float FallGravityMult; //Multiplier to the player's gravityScale when falling.
		public float MaxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
		[Space(5)]
		public float FastFallGravityMult; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
		//Seen in games such as Celeste, lets the player fall extra fast if they wish.
		public float MaxFastFallSpeed; //Maximum fall speed(terminal velocity) of the player when performing a faster fall.

		[Space(20)]

		[Header("Run")]
		public float RunMaxSpeed; //Target speed we want the player to reach.
		public float RunAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
		[HideInInspector] public float RunAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
		public float RunDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
		[HideInInspector] public float RunDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
		[Space(5)]
		[Range(0f, 1)] public float AccelInAir; //Multipliers applied to acceleration rate when airborne.
		[Range(0f, 1)] public float DeccelInAir;
		[Space(5)]
		public bool DoConserveMomentum = true;

		[Space(20)]

		[Header("Jump")]
		public float JumpHeight; //Height of the player's jump
		public float JumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
		[HideInInspector] public float JumpForce; //The actual force applied (upwards) to the player when they jump.

		[Header("Both Jumps")]
		public float JumpCutGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping
		[Range(0f, 1)] public float JumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump
		public float JumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
		[Space(0.5f)]
		public float JumpHangAccelerationMult;
		public float JumpHangMaxSpeedMult;

		[Header("Wall Jump")]
		public Vector2 WallJumpForce; //The actual force (this time set by us) applied to the player when wall jumping.
		[Space(5)]
		[Range(0f, 1f)] public float WallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
		[Range(0f, 1.5f)] public float WallJumpTime; //Time after wall jumping the player's movement is slowed for.
		public bool DoTurnOnWallJump; //Player will rotate to face wall jumping direction

		[Space(20)]

		[Header("Slide")]
		public float SlideSpeed;
		public float SlideAccel;

		[Space(20)]

		[Header("Dash")]
		public int DashAmount;
		public float DashSpeed;
		public float DashSleepTime; //Duration for which the game freezes when we press dash but before we read directional input and apply a force
		[Space(5)]
		public float DashAttackTime;
		[Space(5)]
		public float DashEndTime; //Time after you finish the inital drag phase, smoothing the transition back to idle (or any standard state)
		public Vector2 DashEndSpeed; //Slows down player, makes dash feel more responsive (used in Celeste)
		[Range(0f, 1f)] public float DashEndRunLerp; //Slows the affect of player movement while dashing
		[Space(5)]
		public float DashRefillTime;
		
		[Space(20)]
		
		[Header("Assists")]
		[Range(0.01f, 0.5f)] public float CoyoteTime; //Grace period after falling off a platform, where you can still jump
		[Range(0.01f, 0.5f)] public float JumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.
		[Range(0.01f, 0.5f)] public float DashInputBufferTime;
		
		private void OnValidate()
		{
			//Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
			GravityStrength = -(2 * JumpHeight) / (JumpTimeToApex * JumpTimeToApex);

			//Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
			GravityScale = GravityStrength / Physics2D.gravity.y;

			//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
			RunAccelAmount = (50 * RunAcceleration) / RunMaxSpeed;
			RunDeccelAmount = (50 * RunDecceleration) / RunMaxSpeed;

			//Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
			JumpForce = Mathf.Abs(GravityStrength) * JumpTimeToApex;
			
			RunAcceleration = Mathf.Clamp(RunAcceleration, 0.01f, RunMaxSpeed);
			RunDecceleration = Mathf.Clamp(RunDecceleration, 0.01f, RunMaxSpeed);
		}
	}
}