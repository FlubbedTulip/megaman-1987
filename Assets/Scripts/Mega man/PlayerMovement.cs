using System;
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
    
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 10f;          // Initial jump velocity
        [SerializeField] private float jumpBoost = 20f;          // Upward force applied per second while holding jump
        [SerializeField] private float maxJumpHoldTime = 0.2f;   // How long you can hold jump to keep boosting
        [SerializeField] private float maxUpwardVelocity = 14f;  // Optional: clamp upward speed
        [SerializeField] private float normalGravityScale = 0.2f;

        
        [Header("State Machine")]
        // Optionally store references to states directly in the inspector, or create them in code.
        private IMovementState _onGroundState;
        private IMovementState _inAirState;
        private IMovementState _climbingState;

        private IMovementState _currentState;

        private InputActions _inputActions;
        private Rigidbody2D _rb;
        
        private SpriteRenderer _spriteRenderer;
        private bool _isFacingRight = true;

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

        // For states to access
        public float Speed => speed;
        public float JumpForce => jumpForce;
        public float JumpBoost => jumpBoost;
        public float MaxJumpHoldTime => maxJumpHoldTime;
        public float NormalGravityScale => normalGravityScale;
        
        public float MaxUpwardVelocity => maxUpwardVelocity;
        
        // Movement input cache
        public Vector2 MovementInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool JumpHeld { get; private set; }  
        public bool IsNearLadder { get; set; }
        
        public Rigidbody2D Rb => _rb;



        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _inputActions = new InputActions();

            // Initialize states
            _onGroundState = new OnGroundState();
            _inAirState    = new InAirState();
            _climbingState = new ClimbingState();
            
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();

            // Subscribe to input events
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled  += OnMoveCanceled;

            _inputActions.Player.Jump.started += OnJumpStarted;
            _inputActions.Player.Jump.canceled += OnJumpCanceled;
            

            // Start in OnGroundState by default.
            TransitionToState(_onGroundState);
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();

            // Unsubscribe from input events
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled  -= OnMoveCanceled;

            _inputActions.Player.Jump.started += OnJumpStarted;
            _inputActions.Player.Jump.canceled += OnJumpCanceled;
        }

        private void Update()
        {
            // Each state will handle logic in its Update method
            _currentState.Update(this);
            if (_rb.linearVelocity.x < 0)
            {
                _isFacingRight = false;
            }
            else if (_rb.linearVelocity.x > 0)
            {
                _isFacingRight = true;
            }

            UpdateDiraction();
            
            // Reset JumpPressed so we only handle jump once per frame.
            JumpPressed = false;
        }

        private void UpdateDiraction()
        {
            _spriteRenderer.flipX = !_isFacingRight;
        }


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


        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            MovementInput = Vector2.zero;
        }
        
        
        
        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            // A "one-frame" jump press
            JumpPressed = true;
            // Also set JumpHeld = true when the button is pressed
            JumpHeld = true;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            // The button is released
            JumpHeld = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Ladder"))
            {
                IsNearLadder = true;
            }
        }
    }
}
