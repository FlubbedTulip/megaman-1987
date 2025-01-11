using Interfaces;
using Mega_man.States;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mega_man
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour, IMovementContext
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 10f;
        
        [Header("State Machine")]
        // Optionally store references to states directly in the inspector, or create them in code.
        private IMovementState _onGroundState;
        private IMovementState _inAirState;
        private IMovementState _climbingState;

        private IMovementState _currentState;

        private InputActions _inputActions;
        private Rigidbody2D _rb;

        // The interface property from IMovementContext
        public float GravityScale
        {
            get => _rb.gravityScale;
            set => _rb.gravityScale = value;
        }

        // Expose read-only references to the states so states can do transitions.
        public IMovementState OnGroundState  => _onGroundState;
        public IMovementState InAirState     => _inAirState;
        public IMovementState ClimbingState  => _climbingState;

        // Additional data from context that states might need
        public Rigidbody2D Rb => _rb;
        public float Speed => speed;
        public float JumpForce => jumpForce;
        
        // Movement input cache
        public Vector2 MovementInput { get; private set; }

        // Jump input cache (if you want to store whether jump was pressed this frame)
        public bool JumpPressed { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _inputActions = new InputActions();

            // Initialize states
            _onGroundState = new OnGroundState();
            _inAirState    = new InAirState();
            _climbingState = new ClimbingState();
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();

            // Subscribe to input events
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled  += OnMoveCanceled;

            _inputActions.Player.Jump.performed += OnJumpPerformed;

            // Start in OnGroundState by default (for example).
            TransitionToState(_onGroundState);
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();

            // Unsubscribe from input events
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled  -= OnMoveCanceled;

            _inputActions.Player.Jump.performed -= OnJumpPerformed;
        }

        private void Update()
        {
            // Each state will handle logic in its Update method
            _currentState.Update(this);

            // Reset JumpPressed so we only handle jump once per frame 
            // (if your logic requires it).
            JumpPressed = false;
        }

        /// <summary>
        /// Method used by states to switch from one state to another
        /// </summary>
        public void TransitionToState(IMovementState newState)
        {
            if (_currentState != null)
            {
                _currentState.ExitState(this);
            }

            _currentState = newState;

            if (_currentState != null)
            {
                _currentState.EnterState(this);
            }
        }

        #region Input Callbacks

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            MovementInput = Vector2.zero;
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            // Set a flag indicating jump was requested
            JumpPressed = true;
        }

        #endregion
    }
}
