using Controller2DProject.Controllers;
using Controller2DProject.Controllers.Inputs;
using Sensors2D;
using UnityEngine;
using UnityUtils.Timers;
namespace Controller2DProject.ScriptableStateMachine.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerContext _playerContext;
        public InputReader Input;
        public PlayerData PlayerData;
        public Animator Animator;
        
        [SerializeField] private bool _isInDebugMode;
        public BoxSensorOverlap2D GroundSensor;
        public BoxSensorOverlap2D FrontWallSensor;
        public BoxSensorOverlap2D BackWallSensor;

        public Rigidbody2D Rb;
        private Transform _tr;
        
        public CountdownTimer LastOnGroundTimer;
        public CountdownTimer LastOnWallTimer;
        public CountdownTimer LastOnWallRightTime;
        public CountdownTimer LastOnWallLeftTime;
        public CountdownTimer LastPressedJumpTime;
        public CountdownTimer LastPressedDashTime;
        
        public StateMachine StateMachine;
        
        public bool IsFacingRight { get; private set; }
        public bool IsJumpCut;
        
        private void Awake()
        {
            _tr = transform;
            Rb = GetComponent<Rigidbody2D>();

            LastOnGroundTimer = new CountdownTimer(0f, this);
            LastOnWallTimer = new CountdownTimer(0f, this);
            LastOnWallRightTime  = new CountdownTimer(0f, this);
            LastOnWallLeftTime   = new CountdownTimer(0f, this);
            LastPressedJumpTime = new CountdownTimer(0f, this);
            LastPressedDashTime = new CountdownTimer(0f, this);
            
            _playerContext.Initialize(this);
        }
        
        private void Start()
        {
            Input.EnablePlayerActions();

            SetGravityScale(PlayerData.GravityScale);
            IsFacingRight = true;
            StateMachine.Initialize();
        }

        private void OnEnable()
        {
            Input.Move += CheckDirectionToFace;
            Input.Jump += OnJumpInput;
            Input.Dash += OnDashInput;
        }

        private void OnDisable()
        {
            Input.Move -= CheckDirectionToFace;
            Input.Jump -= OnJumpInput;
            Input.Dash -= OnDashInput;
        }

        private void CheckDirectionToFace(Vector2 direction)
        {
            if (!HaveHorizontalInput()) return;
            
            bool wantsToFaceRight = direction.x > 0;
            if (wantsToFaceRight != IsFacingRight)
                Turn(wantsToFaceRight);
        }

        private void OnJumpInput(bool jumpInputStarted)
        {
            if (jumpInputStarted)
                LastPressedJumpTime.Restart(PlayerData.JumpInputBufferTime);
        }

        private void OnDashInput()
        {
            LastPressedDashTime.Restart(PlayerData.DashInputBufferTime);
        }
        
        private void Turn(bool faceRight)
        {
            IsFacingRight = faceRight;
            Vector3 scale = _tr.localScale;
            scale.x = Mathf.Abs(scale.x) * (IsFacingRight ? 1 : -1);
            _tr.localScale = scale;
        }
        
        private void Update()
        {
            UpdateWallAndGroundSensors();
            StateMachine.Update();
        }

        private void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }
        
        private void UpdateWallAndGroundSensors()
        {
            GroundSensor.Cast();
            FrontWallSensor.Cast();
            BackWallSensor.Cast();
            
            if (GroundSensor.HasDetectedHit())
                LastOnGroundTimer.Restart(PlayerData.CoyoteTime);
            
            if (GroundSensor.HasDetectedHit() && StateMachine.CurrentState is not JumpStateScriptable)
                LastOnGroundTimer.Restart(PlayerData.CoyoteTime);
            
            // if (IsWallOnRight() && _stateMachine.CurrentState is not WallJumpState)
            //     LastOnWallRightTime.Restart(_playerData.CoyoteTime);
            //
            // if (IsWallOnLeft() && _stateMachine.CurrentState is not WallJumpState)
            //     LastOnWallLeftTime.Restart(_playerData.CoyoteTime);
            
            float maxWallTime = Mathf.Max(LastOnWallLeftTime.CurrentTime, LastOnWallRightTime.CurrentTime);
            if (maxWallTime > 0)
                LastOnWallTimer.Restart(maxWallTime);
        }

        public void SetGravityScale(float scale)
        {
            Rb.gravityScale = scale;
        }
        
        private bool HaveHorizontalInput()
        {
            return Mathf.Abs(Input.Direction.x) > 0.01f;
        }
        
        private bool IsWallOnRight()
        {
            return (FrontWallSensor.HasDetectedHit() && IsFacingRight) ||
                (BackWallSensor.HasDetectedHit() && !IsFacingRight);
        }

        private bool IsWallOnLeft()
        {
            return (FrontWallSensor.HasDetectedHit() && !IsFacingRight) ||
                (BackWallSensor.HasDetectedHit() && IsFacingRight);
        }
        
        private void OnDrawGizmos()
        {
            if (!_isInDebugMode) return;
            GroundSensor.DrawGizmos();
            FrontWallSensor.DrawGizmos();
            BackWallSensor.DrawGizmos();
        }
    }
}