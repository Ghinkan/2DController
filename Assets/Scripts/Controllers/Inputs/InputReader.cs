using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
namespace Controller2DProject.Controllers.Inputs
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Controller2D/InputReader", order = 0)]
    public class InputReader : ScriptableObject, PlayerInputs.IPlayerActions
    {
        public event UnityAction<Vector2> Move;
        public event UnityAction<Vector2> Look;
        public event UnityAction<bool> Jump;
        public event UnityAction Dash;
        
        public Vector2 Direction => _playerControls.Player.Move.ReadValue<Vector2>();
        public Vector2 LookDirection => _playerControls.Player.Look.ReadValue<Vector2>();
        public bool IsJumpKeyPressed() => _playerControls.Player.Jump.IsPressed();
        
        private PlayerInputs _playerControls;
        
        public void EnablePlayerActions()
        {
            if (_playerControls == null)
            {
                _playerControls = new PlayerInputs();
                _playerControls.Player.SetCallbacks(this);
            }
            
            _playerControls.Enable();
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            Move?.Invoke(context.ReadValue<Vector2>());
        }
        
        public void OnLook(InputAction.CallbackContext context)
        {
            Look?.Invoke(context.ReadValue<Vector2>());
        }
        
        public void OnAttack(InputAction.CallbackContext context)
        {
            
        }
        
        public void OnInteract(InputAction.CallbackContext context)
        {
            
        }
        
        public void OnCrouch(InputAction.CallbackContext context)
        {
            
        }
        
        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Jump?.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump?.Invoke(false);
                    break;
            }
        }
        public void OnDash(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Started)
                Dash?.Invoke();
        }
    }
}