using System;
using System.Collections;
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

        public bool IsFacingRight { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsWallJumping { get; private set; }
        public bool IsDashing { get; private set; }

        //Timers
        public CountdownTimer LastOnGroundTimer;
        public CountdownTimer LastOnWallTimer;
        public CountdownTimer LastOnWallRightTime;
        public CountdownTimer LastOnWallLeftTime;
        public CountdownTimer LastPressedJumpTime;
        private CountdownTimer _lastPressedDashTime;

        //Jump
        public bool IsJumpCut;

        //Wall Jump
        private float _wallJumpStartTime;
        private int _lastWallJumpDir;

        //Dash
        private int _dashesLeft;
        private bool _dashRefilling;
        private Vector2 _lastDashDir;
        private bool _isDashAttacking;

        private StateMachine _stateMachine;
        private IdleState _idleState;
        private RunState _runState;
        private JumpState _jumpState;
        private FallState _fallState;
        private WallSlide _wallSlide;
        private WallJumpState _wallJumpState;

        private void Awake()
        {
            _tr = transform;
            _rb = GetComponent<Rigidbody2D>();

            LastOnGroundTimer = new CountdownTimer(0f, this);
            LastOnWallTimer = new CountdownTimer(0f, this);
            LastOnWallRightTime  = new CountdownTimer(0f, this);
            LastOnWallLeftTime   = new CountdownTimer(0f, this);
            LastPressedJumpTime = new CountdownTimer(0f, this);
            _lastPressedDashTime = new CountdownTimer(0f, this);
            
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

            At(_idleState, _runState, () => _input.Direction.x != 0);
            At(_idleState, _jumpState,    () => CanJump() && LastPressedJumpTime.IsRunning);
            At(_idleState, _fallState,    () => LastOnGroundTimer.IsFinished);
            
            At(_runState, _idleState,     () => Mathf.Abs(_rb.linearVelocityX) <= 0 && _input.Direction.x == 0);
            At(_runState, _jumpState,     () => CanJump() && LastPressedJumpTime.IsRunning);
            At(_runState, _fallState,     () => LastOnGroundTimer.IsFinished);
            
            At(_jumpState, _fallState,    () => _rb.linearVelocityY < 0);
            At(_jumpState, _idleState,    () => IsGrounded() && Mathf.Abs(_rb.linearVelocityX) <= 0);
            At(_jumpState, _runState,     () => IsGrounded() && _input.Direction.x != 0);
            At(_wallSlide, _wallJumpState, () => CanWallJump() && LastPressedJumpTime.IsRunning);
            
            At(_fallState, _idleState,    () => IsGrounded() && Mathf.Abs(_rb.linearVelocityX) <= 0);
            At(_fallState, _runState,     () => IsGrounded() && _input.Direction.x != 0);
            At(_fallState, _wallSlide, () => CanSlide() && ((LastOnWallLeftTime.IsRunning && _input.Direction.x < 0) || (LastOnWallRightTime.IsRunning && _input.Direction.x > 0)));
            At(_fallState, _wallJumpState, () => CanWallJump() && LastPressedJumpTime.IsRunning);
            
            At(_wallSlide, _idleState, () => IsGrounded() && Mathf.Abs(_rb.linearVelocityX) <= 0);
            At(_wallSlide, _runState, () => IsGrounded() && _input.Direction.x != 0);
            At(_wallSlide, _fallState, () => !(CanSlide() && ((LastOnWallLeftTime.IsRunning && _input.Direction.x < 0) || (LastOnWallRightTime.IsRunning && _input.Direction.x > 0))));
            At(_wallSlide, _wallJumpState, () => CanWallJump() && LastPressedJumpTime.IsRunning);
            
            At(_wallJumpState, _fallState,    () => _rb.linearVelocityY < 0);
            At(_wallJumpState, _idleState,    () => IsGrounded() && Mathf.Abs(_rb.linearVelocityX) <= 0);
            At(_wallJumpState, _runState,     () => IsGrounded() && _input.Direction.x != 0);
            
            _stateMachine.SetState(_idleState);
        }
        
        private void At(IState from, IState to, Func<bool> condition)
        {
            _stateMachine.AddTransition(from, to, condition);
        }
        
        private void Any<T>(IState to, Func<bool> condition)
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
            _lastPressedDashTime.Restart(_playerData.DashInputBufferTime);
        }
        
        private void Turn(bool faceRight)
        {
            IsFacingRight = faceRight;

            Vector3 scale = _tr.localScale;
            scale.x = Mathf.Abs(scale.x) * (IsFacingRight ? 1 : -1);
            _tr.localScale = scale;
        }

        private bool IsGrounded()
        {
            _groundSensor.Cast();
            return _groundSensor.HasDetectedHit();
        }
        
        private void Update()
        {
            _stateMachine.Update();
        }

        private void FixedUpdate()
        {
            //CollisionsChecks
            if (!IsDashing && !IsJumping)
            {
                _groundSensor.Cast();
                _frontWallSensor.Cast();
                _backWallSensor.Cast();
            
                if (!IsDashing && !IsJumping)
                {
                    if (_groundSensor.HasDetectedHit())
                    {
                        LastOnGroundTimer.Restart(_playerData.CoyoteTime);
                    }
            
                    //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
                    if (((_frontWallSensor.HasDetectedHit() && IsFacingRight) || (_backWallSensor.HasDetectedHit() && !IsFacingRight)) && !IsWallJumping)
                    {
                        LastOnWallRightTime.Restart(_playerData.CoyoteTime);
                    }
            
                    if (((_frontWallSensor.HasDetectedHit() && !IsFacingRight) || (_backWallSensor.HasDetectedHit() && IsFacingRight)) && !IsWallJumping)
                    {
                        LastOnWallLeftTime.Restart(_playerData.CoyoteTime);
                    }
                    
                    float maxWallTime = Mathf.Max(LastOnWallLeftTime.CurrentTime, LastOnWallRightTime.CurrentTime);
                    if(maxWallTime > 0)
                        LastOnWallTimer.Restart(maxWallTime);
                }
            }
            
            _stateMachine.FixedUpdate();
        }

        public void SetGravityScale(float scale)
        {
            _rb.gravityScale = scale;
        }

        private void Sleep(float duration)
        {
            //Method used so we don't need to call StartCoroutine everywhere
            //nameof() notation means we don't need to input a string directly.
            //Removes chance of spelling mistakes and will improve error messages if any
            StartCoroutine(nameof(PerformSleep), duration);
        }

        private IEnumerator PerformSleep(float duration)
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
            Time.timeScale = 1;
        }
        
        private IEnumerator StartDash(Vector2 dir)
        {
            //Overall this method of dashing aims to mimic Celeste, if you're looking for
            // a more physics-based approach try a method similar to that used in the jump
            
            LastOnGroundTimer.Stop();
            _lastPressedDashTime.Stop();

            float startTime = Time.time;

            _dashesLeft--;
            _isDashAttacking = true;

            SetGravityScale(0);

            //We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
            while (Time.time - startTime <= _playerData.DashAttackTime)
            {
                _rb.linearVelocity = dir.normalized * _playerData.DashSpeed;
                //Pauses the loop until the next frame, creating something of a Update loop. 
                //This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
                yield return null;
            }

            startTime = Time.time;

            _isDashAttacking = false;

            //Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
            SetGravityScale(_playerData.GravityScale);
            _rb.linearVelocity = _playerData.DashEndSpeed * dir.normalized;

            while (Time.time - startTime <= _playerData.DashEndTime)
            {
                yield return null;
            }

            //Dash over
            IsDashing = false;
        }

        //Short period before the player is able to dash again
        private IEnumerator RefillDash(int amount)
        {
            //SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
            _dashRefilling = true;
            yield return new WaitForSeconds(_playerData.DashRefillTime);
            _dashRefilling = false;
            _dashesLeft = Mathf.Min(_playerData.DashAmount, _dashesLeft + 1);
        }
        
        private bool CanJump()
        {
            return LastOnGroundTimer.IsRunning && !IsJumping;
        }

        private bool CanWallJump()
        {
            return LastPressedJumpTime.IsRunning && LastOnWallTimer.IsRunning && LastOnGroundTimer.IsFinished && (!IsWallJumping ||
                (LastOnWallRightTime.IsRunning && _lastWallJumpDir == 1) || (LastOnWallLeftTime.IsRunning && _lastWallJumpDir == -1));
        }

        private bool CanDash()
        {
            if (!IsDashing && _dashesLeft < _playerData.DashAmount && LastOnGroundTimer.IsRunning && !_dashRefilling)
            {
                StartCoroutine(nameof(RefillDash), 1);
            }

            return _dashesLeft > 0;
        }

        private bool CanSlide()
        {
            return LastOnWallTimer.IsRunning && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTimer.IsFinished;
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