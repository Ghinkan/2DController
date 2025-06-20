using System;
using System.Collections;
using Controller2DProject.Controllers.Inputs;
using Sensors2D;
using UnityEngine;
using UnityUtils.Timers;
namespace Controller2DProject.Controllers
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        [SerializeField] private PlayerData _playerData;
        
        [SerializeField] private bool _isInDebugMode;
        [SerializeField] private BoxSensorOverlap2D _groundSensor;
        [SerializeField] private BoxSensorOverlap2D _frontWallSensor;
        [SerializeField] private BoxSensorOverlap2D _backWallSensor;

        private Rigidbody2D _rb;
        private Transform _tr;

        //Variables control the various actions the player can perform at any time.
        //These are fields which can are public allowing for other sctipts to read them
        //but can only be privately written to.
        public bool IsFacingRight { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsWallJumping { get; private set; }
        public bool IsDashing { get; private set; }
        public bool IsSliding { get; private set; }

        //Timers
        private CountdownTimer _lastOnGroundTimer;
        private CountdownTimer _lastOnWallTimer;
        private CountdownTimer _lastOnWallRightTime;
        private CountdownTimer _lastOnWallLeftTime;
        private CountdownTimer _lastPressedJumpTime;
        private CountdownTimer _lastPressedDashTime;

        //Jump
        private bool _isJumpCut;
        private bool _isJumpFalling;

        //Wall Jump
        private float _wallJumpStartTime;
        private int _lastWallJumpDir;

        //Dash
        private int _dashesLeft;
        private bool _dashRefilling;
        private Vector2 _lastDashDir;
        private bool _isDashAttacking;

        private void Awake()
        {
            _tr = transform;
            _rb = GetComponent<Rigidbody2D>();

            _lastOnGroundTimer = new CountdownTimer(0f, this);
            _lastOnWallTimer = new CountdownTimer(0f, this);
            _lastOnWallRightTime  = new CountdownTimer(0f, this);
            _lastOnWallLeftTime   = new CountdownTimer(0f, this);
            _lastPressedJumpTime = new CountdownTimer(0f, this);
            _lastPressedDashTime = new CountdownTimer(0f, this);
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
                _lastPressedJumpTime.Restart(_playerData.JumpInputBufferTime);
            else
            {
                if (CanJumpCut() || CanWallJumpCut())
                    _isJumpCut = true;
            }
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
        private void Update()
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
                        _lastOnGroundTimer.Restart(_playerData.CoyoteTime);
                    }

                    //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
                    if (((_frontWallSensor.HasDetectedHit() && IsFacingRight) || (_backWallSensor.HasDetectedHit() && !IsFacingRight)) && !IsWallJumping)
                    {
                        _lastOnWallRightTime.Restart(_playerData.CoyoteTime);
                    }

                    if (((_frontWallSensor.HasDetectedHit() && !IsFacingRight) || (_backWallSensor.HasDetectedHit() && IsFacingRight)) && !IsWallJumping)
                    {
                        _lastOnWallLeftTime.Restart(_playerData.CoyoteTime);
                    }
                    
                    float maxWallTime = Mathf.Max(_lastOnWallLeftTime.CurrentTime, _lastOnWallRightTime.CurrentTime);
                    if(maxWallTime > 0)
                        _lastOnWallTimer.Restart(maxWallTime);
                }
            }

            //JumpChecks
            if (IsJumping && _rb.linearVelocity.y < 0)
            {
                IsJumping = false;

                if (!IsWallJumping)
                    _isJumpFalling = true;
            }

            if (IsWallJumping && Time.time - _wallJumpStartTime > _playerData.WallJumpTime)
            {
                IsWallJumping = false;
            }

            if (_lastOnGroundTimer.IsRunning && !IsJumping && !IsWallJumping)
            {
                _isJumpCut = false;

                if (!IsJumping)
                    _isJumpFalling = false;
            }

            if (!IsDashing)
            {
                //Jump
                if (CanJump() && _lastPressedJumpTime.IsRunning)
                {
                    IsJumping = true;
                    IsWallJumping = false;
                    _isJumpCut = false;
                    _isJumpFalling = false;
                    Jump();
                }
                //WALL JUMP
                else if (CanWallJump() && _lastPressedJumpTime.IsRunning)
                {
                    IsWallJumping = true;
                    IsJumping = false;
                    _isJumpCut = false;
                    _isJumpFalling = false;
                
                    _wallJumpStartTime = Time.time;
                    _lastWallJumpDir = (_lastOnWallRightTime.IsRunning) ? -1 : 1;
                
                    WallJump(_lastWallJumpDir);
                }
            }

            //DashChecks
            if (CanDash() && _lastPressedDashTime.IsRunning)
            {
                //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
                Sleep(_playerData.DashSleepTime);

                //If not direction pressed, dash forward
                if (_input.Direction != Vector2.zero)
                    _lastDashDir = _input.Direction;
                else
                    _lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;
                
                IsDashing = true;
                IsJumping = false;
                IsWallJumping = false;
                _isJumpCut = false;

                StartCoroutine(nameof(StartDash), _lastDashDir);
            }

            //SlideChecks
            if (CanSlide() && ((_lastOnWallLeftTime.IsRunning && _input.Direction.x < 0) || (_lastOnWallRightTime.IsRunning && _input.Direction.x > 0)))
                IsSliding = true;
            else
                IsSliding = false;

            //Gravity
            if (!_isDashAttacking)
            {
                //Higher gravity if we've released the jump input or are falling
                if (IsSliding)
                {
                    SetGravityScale(0);
                }
                else if (_rb.linearVelocity.y < 0 && _input.Direction.y < 0)
                {
                    //Much higher gravity if holding down
                    SetGravityScale(_playerData.GravityScale * _playerData.FastFallGravityMult);
                    //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                    _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -_playerData.MaxFastFallSpeed));
                }
                else if (_isJumpCut)
                {
                    //Higher gravity if jump button released
                    SetGravityScale(_playerData.GravityScale * _playerData.JumpCutGravityMult);
                    _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -_playerData.MaxFallSpeed));
                }
                else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(_rb.linearVelocity.y) < _playerData.JumpHangTimeThreshold)
                {
                    SetGravityScale(_playerData.GravityScale * _playerData.JumpHangGravityMult);
                }
                else if (_rb.linearVelocity.y < 0)
                {
                    //Higher gravity if falling
                    SetGravityScale(_playerData.GravityScale * _playerData.FallGravityMult);
                    //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                    _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -_playerData.MaxFallSpeed));
                }
                else
                {
                    //Default gravity if standing on a platform or moving upwards
                    SetGravityScale(_playerData.GravityScale);
                }
            }
            else
            {
                //No gravity when dashing (returns to normal once initial dashAttack phase over)
                SetGravityScale(0);
            }
        }

        private void FixedUpdate()
        {
            //Handle Run
            if (!IsDashing)
            {
                if (IsWallJumping)
                    Run(_playerData.WallJumpRunLerp);
                else
                    Run(1);
            }
            else if (_isDashAttacking)
            {
                Run(_playerData.DashEndRunLerp);
            }

            //Handle Slide
            if (IsSliding)
                Slide();
        }

        private void SetGravityScale(float scale)
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

        private void Run(float lerpAmount)
        {
            //Calculate the direction we want to move in and our desired velocity
            float targetSpeed = _input.Direction.x * _playerData.RunMaxSpeed;
            //We can reduce are control using Lerp() this smooths changes to are direction and speed
            targetSpeed = Mathf.Lerp(_rb.linearVelocity.x, targetSpeed, lerpAmount);
            
            //Gets an acceleration value based on if we are accelerating (includes turning) 
            //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
            float accelRate;
            if (_lastOnGroundTimer.IsRunning)
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _playerData.RunAccelAmount : _playerData.RunDeccelAmount;
            else
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _playerData.RunAccelAmount * _playerData.AccelInAir : _playerData.RunDeccelAmount * _playerData.DeccelInAir;
            
            //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
            if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(_rb.linearVelocity.y) < _playerData.JumpHangTimeThreshold)
            {
                accelRate *= _playerData.JumpHangAccelerationMult;
                targetSpeed *= _playerData.JumpHangMaxSpeedMult;
            }
            
            //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
            if (_playerData.DoConserveMomentum && Mathf.Abs(_rb.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(_rb.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && _lastOnGroundTimer.IsFinished)
            {
                //Prevent any deceleration from happening, or in other words conserve are current momentum
                //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
                accelRate = 0;
            }
            
            //Calculate difference between current velocity and desired velocity
            //Calculate force along x-axis to apply to thr player
            float speedDif = targetSpeed - _rb.linearVelocity.x;
            float movement = speedDif * accelRate;

            //Convert this to a vector and apply to rigidbody
            _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
        
        private void Jump()
        {
            //Ensures we can't call Jump multiple times from one press
            _lastPressedJumpTime.Stop();
            _lastOnGroundTimer.Stop();
            
            //We increase the force applied if we are falling
            //This means we'll always feel like we jump the same amount 
            //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
            float force = _playerData.JumpForce;
            if (_rb.linearVelocity.y < 0)
                force -= _rb.linearVelocity.y;

            _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }
        
        private void WallJump(int dir)
        {
            //Ensures we can't call Wall Jump multiple times from one press
            _lastPressedJumpTime.Stop();
            _lastOnGroundTimer.Stop();
            _lastOnWallRightTime.Stop();
            _lastOnWallLeftTime.Stop();
            
            Vector2 force = new Vector2(_playerData.WallJumpForce.x, _playerData.WallJumpForce.y);
            force.x *= dir; //apply force in opposite direction of wall

            if (Mathf.Sign(_rb.linearVelocity.x) != Mathf.Sign(force.x))
                force.x -= _rb.linearVelocity.x;

            if (_rb.linearVelocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
                force.y -= _rb.linearVelocity.y;

            //Unlike in the run we want to use the Impulse mode.
            //The default mode will apply are force instantly ignoring masss
            _rb.AddForce(force, ForceMode2D.Impulse);
        }
        
        private IEnumerator StartDash(Vector2 dir)
        {
            //Overall this method of dashing aims to mimic Celeste, if you're looking for
            // a more physics-based approach try a method similar to that used in the jump
            
            _lastOnGroundTimer.Stop();
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
        
        private void Slide()
        {
            //Works the same as the Run but only in the y-axis
            //THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
            float speedDif = _playerData.SlideSpeed - _rb.linearVelocity.y;	
            float movement = speedDif * _playerData.SlideAccel;
            //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
            //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
            movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

            _rb.AddForce(movement * Vector2.up);
        }
        
        private bool CanJump()
        {
            return _lastOnGroundTimer.IsRunning && !IsJumping;
        }

        private bool CanWallJump()
        {
            return _lastPressedJumpTime.IsRunning && _lastOnWallTimer.IsRunning && _lastOnGroundTimer.IsFinished && (!IsWallJumping ||
                (_lastOnWallRightTime.IsRunning && _lastWallJumpDir == 1) || (_lastOnWallLeftTime.IsRunning && _lastWallJumpDir == -1));
        }

        private bool CanJumpCut()
        {
            return IsJumping && _rb.linearVelocity.y > 0;
        }

        private bool CanWallJumpCut()
        {
            return IsWallJumping && _rb.linearVelocity.y > 0;
        }

        private bool CanDash()
        {
            if (!IsDashing && _dashesLeft < _playerData.DashAmount && _lastOnGroundTimer.IsRunning && !_dashRefilling)
            {
                StartCoroutine(nameof(RefillDash), 1);
            }

            return _dashesLeft > 0;
        }

        private bool CanSlide()
        {
            if (_lastOnWallTimer.IsRunning && !IsJumping && !IsWallJumping && !IsDashing && _lastOnGroundTimer.IsFinished)
                return true;
            else
                return false;
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