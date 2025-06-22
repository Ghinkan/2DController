using System;
using Controller2DProject.Controllers.Inputs;
using Controller2DProject.Controllers.States;
using Sensors2D;
using UnityEngine;
using UnityUtils.StateMachine;
using UnityUtils.Timers;
namespace Controller2DProject.Controllers
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerControllerStates : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        [SerializeField] private PlayerData _playerData;
        
        [SerializeField] private bool _isInDebugMode;
        [SerializeField] private BoxSensorOverlap2D _groundSensor;
        [SerializeField] private BoxSensorOverlap2D _frontWallSensor;
        [SerializeField] private BoxSensorOverlap2D _backWallSensor;

        private Rigidbody2D _rb;
        private Transform _tr;
        
        public CountdownTimer LastOnGroundTimer;
        public CountdownTimer LastOnWallTimer;
        public CountdownTimer LastOnWallRightTime;
        public CountdownTimer LastOnWallLeftTime;
        public CountdownTimer LastPressedJumpTime;
        public CountdownTimer LastPressedDashTime;
        
        private StateMachine _stateMachine;
        private IdleState _idleState;
        private RunState _runState;
        private JumpState _jumpState;
        private FallState _fallState;
        private WallSlide _wallSlide;
        private WallJumpState _wallJumpState;
        private DashState _dashState;
        
        public bool IsFacingRight { get; private set; }
        
        private void Awake()
        {
            _tr = transform;
            _rb = GetComponent<Rigidbody2D>();

            LastOnGroundTimer = new CountdownTimer(0f, this);
            LastOnWallTimer = new CountdownTimer(0f, this);
            LastOnWallRightTime  = new CountdownTimer(0f, this);
            LastOnWallLeftTime   = new CountdownTimer(0f, this);
            LastPressedJumpTime = new CountdownTimer(0f, this);
            LastPressedDashTime = new CountdownTimer(0f, this);
            
            SetupStateMachine();
        }

        private void SetupStateMachine()
        {
            _stateMachine = new StateMachine();
            
            _idleState = new IdleState(this, _input, _playerData, _rb);
            _runState = new RunState(this, _input, _playerData, _rb);
            _jumpState = new JumpState(this, _input, _playerData, _rb);
            _fallState = new FallState(this,_input, _playerData, _rb);
            _wallSlide = new WallSlide(this, _input, _playerData, _rb);
            _wallJumpState = new WallJumpState(this, _input, _playerData, _rb);
            _dashState = new DashState(this, _input, _playerData, _rb);

            At(_idleState, _runState, () => IsGrounded() && HaveHorizontalInput());
            At(_idleState, _jumpState,    () => CanJump());
            At(_idleState, _fallState,    () => LastOnGroundTimer.IsFinished);
            
            At(_runState, _idleState,     () => IsIdle() && !HaveHorizontalInput());
            At(_runState, _jumpState,     () => CanJump());
            At(_runState, _fallState,     () => LastOnGroundTimer.IsFinished);
            
            At(_jumpState, _fallState,    () => IsFalling());
            At(_jumpState, _idleState,    () => IsGrounded() && IsIdle());
            At(_jumpState, _runState,     () => IsGrounded() && HaveHorizontalInput());
            At(_wallSlide, _wallJumpState, () => CanWallJump());
            
            At(_fallState, _idleState,    () => IsGrounded() && IsIdle());
            At(_fallState, _runState,     () => IsGrounded() && HaveHorizontalInput());
            At(_fallState, _wallSlide, () => CanSlide());
            At(_fallState, _wallJumpState, () => CanWallJump());
            
            At(_wallSlide, _idleState, () => IsGrounded() && IsIdle());
            At(_wallSlide, _runState, () => IsGrounded() && HaveHorizontalInput());
            At(_wallSlide, _fallState, () => !CanSlide());
            At(_wallSlide, _wallJumpState, () => CanWallJump());
            
            At(_wallJumpState, _fallState,    () => IsFalling());
            At(_wallJumpState, _idleState,    () => IsGrounded() && IsIdle());
            At(_wallJumpState, _runState,     () => IsGrounded() && HaveHorizontalInput());
            
            Any(_dashState, () => _dashState.DashesLeft > 0 && LastPressedDashTime.IsRunning);
            At(_dashState, _runState, () => !_dashState.IsDashing && IsGrounded());
            At(_dashState, _fallState, () => !_dashState.IsDashing && (LastOnGroundTimer.IsFinished));
            At(_dashState, _wallSlide, () => !_dashState.IsDashing && CanSlide());
            
            _stateMachine.SetState(_idleState);
        }
        
        private void At(IState from, IState to, Func<bool> condition)
        {
            _stateMachine.AddTransition(from, to, condition);
        }
        
        private void Any(IState to, Func<bool> condition)
        {
            _stateMachine.AddAnyTransition(to, condition);
        }

        private void Start()
        {
            _input.EnablePlayerActions();

            SetGravityScale(_playerData.GravityScale);
            IsFacingRight = true;
        }

        private void OnEnable()
        {
            _input.Move += CheckDirectionToFace;
            _input.Jump += OnJumpInput;
            _input.Dash += OnDashInput;
        }

        private void OnDisable()
        {
            _input.Move -= CheckDirectionToFace;
            _input.Jump -= OnJumpInput;
            _input.Dash -= OnDashInput;
        }

        private void CheckDirectionToFace(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) < Mathf.Epsilon) return;

            bool wantsToFaceRight = direction.x > 0;
            if (wantsToFaceRight != IsFacingRight)
                Turn(wantsToFaceRight);
        }

        private void OnJumpInput(bool jumpInputStarted)
        {
            if (jumpInputStarted)
                LastPressedJumpTime.Restart(_playerData.JumpInputBufferTime);
        }

        private void OnDashInput()
        {
            LastPressedDashTime.Restart(_playerData.DashInputBufferTime);
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
            _stateMachine.Update();
        }

        private void FixedUpdate()
        {
            UpdateWallAndGroundSensors();
            _stateMachine.FixedUpdate();
        }
        
        private void UpdateWallAndGroundSensors()
        {
            _groundSensor.Cast();
            _frontWallSensor.Cast();
            _backWallSensor.Cast();

            if (_groundSensor.HasDetectedHit())
                LastOnGroundTimer.Restart(_playerData.CoyoteTime);
            
            if (IsWallOnRight())
                LastOnWallRightTime.Restart(_playerData.CoyoteTime);

            if (IsWallOnLeft())
                LastOnWallLeftTime.Restart(_playerData.CoyoteTime);

            float maxWallTime = Mathf.Max(LastOnWallLeftTime.CurrentTime, LastOnWallRightTime.CurrentTime);
            if (maxWallTime > 0)
                LastOnWallTimer.Restart(maxWallTime);
        }

        public void SetGravityScale(float scale)
        {
            _rb.gravityScale = scale;
        }
        
        private bool IsIdle()
        {
            return Mathf.Abs(_rb.linearVelocityX) <= 0f;
        }

        private bool HaveHorizontalInput()
        {
            return Mathf.Abs(_input.Direction.x) > 0.01f;
        }

        private bool IsFalling()
        {
            return _rb.linearVelocityY < 0f;
        }
        
        private bool IsGrounded()
        {
            _groundSensor.Cast();
            return _groundSensor.HasDetectedHit();
        }
        
        private bool IsWallOnRight()
        {
            return (_frontWallSensor.HasDetectedHit() && IsFacingRight) ||
                (_backWallSensor.HasDetectedHit() && !IsFacingRight);
        }

        private bool IsWallOnLeft()
        {
            return (_frontWallSensor.HasDetectedHit() && !IsFacingRight) ||
                (_backWallSensor.HasDetectedHit() && IsFacingRight);
        }
        
        private bool CanJump()
        {
            return LastOnGroundTimer.IsRunning && LastPressedJumpTime.IsRunning;
        }

        private bool CanWallJump()
        {
            return LastPressedJumpTime.IsRunning && LastOnWallTimer.IsRunning && LastOnGroundTimer.IsFinished;
        }
        
        private bool CanSlide()
        {
            return LastOnWallTimer.IsRunning && LastOnGroundTimer.IsFinished &&
                ((LastOnWallLeftTime.IsRunning && _input.Direction.x < 0) || (LastOnWallRightTime.IsRunning && _input.Direction.x > 0));;
        }

        private void OnDrawGizmos()
        {
            if (!_isInDebugMode) return;
            _groundSensor.DrawGizmos();
            _frontWallSensor.DrawGizmos();
            _backWallSensor.DrawGizmos();
        }
    }
}