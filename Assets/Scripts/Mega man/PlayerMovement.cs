using Interfaces;
using Mega_man.States;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mega_man
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class PlayerMovement : MonoBehaviour, IMovementContext
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 5f;

        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 10f;        // Initial jump velocity
        [SerializeField] private float jumpBoost = 20f;        // Upward force per second while holding jump
        [SerializeField] private float maxJumpHoldTime = 0.2f; // How long you can hold jump
        [SerializeField] private float maxUpwardVelocity = 14f;// Optional velocity clamp
        [SerializeField] private float normalGravityScale = 1f;
        
       

        // States
        private IMovementState _groundedState;
        private IMovementState _inAirState;
        private IMovementState _climbingState;
        private IMovementState _currentState;

        // Input
        private InputActions _inputActions;

        // Cached components
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private PlayerShoot _playerShooting;
        private PlayerAnimationController _animController;


        // IMovementContext property
        public float GravityScale
        {
            get => _rb.gravityScale;
            set => _rb.gravityScale = value;
        }

        // Expose states (used inside states for transitions)
        public IMovementState GroundedState => _groundedState;
        public IMovementState InAirState   => _inAirState;
        public IMovementState ClimbingState=> _climbingState;

        // Movement parameters read by states
        public float Speed            => speed;
        public float JumpForce        => jumpForce;
        public float JumpBoost        => jumpBoost;
        public float MaxJumpHoldTime  => maxJumpHoldTime;
        public float MaxUpwardVelocity=> maxUpwardVelocity;
        public float NormalGravityScale => normalGravityScale;
        
        
        public PlayerAnimationController Anim => _animController;


        // Public properties for user input
        public Vector2 MovementInput  { get; private set; }
        public bool    JumpPressed    { get; private set; }
        public bool    JumpHeld       { get; private set; }
        public bool    IsNearLadder   { get; set; }
        private bool IsFacingRight { get; set; } = true; 
        
        
        

        public Rigidbody2D Rb => _rb;

        

        private void Awake()
        {
            // Components
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _playerShooting = GetComponent<PlayerShoot>();
            _animController = GetComponent<PlayerAnimationController>();


            // Input
            _inputActions = new InputActions();

            // States
            _groundedState = new GroundedState();
            _inAirState    = new InAirState();
            _climbingState = new ClimbingState();
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();

            // Subscribe input
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled  += OnMoveCanceled;
            _inputActions.Player.Jump.started   += OnJumpStarted;
            _inputActions.Player.Jump.canceled  += OnJumpCanceled;
            _inputActions.Player.Shoot.started += OnShootStarted;


            // Start in grounded
            TransitionToState(_groundedState);
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();

            // Unsubscribe properly ( -= )
            _inputActions.Player.Move.performed  -= OnMovePerformed;
            _inputActions.Player.Move.canceled   -= OnMoveCanceled;
            _inputActions.Player.Jump.started    -= OnJumpStarted;
            _inputActions.Player.Jump.canceled   -= OnJumpCanceled;
            _inputActions.Player.Shoot.started -= OnShootStarted;

        }

        private void Update()
        {
            // Let the current state handle logic
            _currentState.UpdateState(this);

            // Face direction
            UpdateFacingDirection();

            // Reset JumpPressed so it's one-frame only
            JumpPressed = false;
        }

        private void UpdateFacingDirection()
        {
            // if velocity.x < 0 => face left, if velocity.x > 0 => face right
            float vx = _rb.linearVelocity.x;
            if (vx < -0.01f)  IsFacingRight = false;
            if (vx >  0.01f)  IsFacingRight = true;

            _spriteRenderer.flipX = !IsFacingRight;
        }

        public void TransitionToState(IMovementState newState)
        {
            _currentState?.ExitState(this);
            _currentState = newState;
            _currentState?.EnterState(this);
        }

        // Input Callbacks
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
            JumpPressed = true;
            JumpHeld = true;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            JumpHeld = false;
        }
        
        private void OnShootStarted(InputAction.CallbackContext context)
        {
            _animController.SetShooting();
            _playerShooting.Shoot(IsFacingRight);
        }

        // Example: Ladder trigger detection
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Ladder"))
            {
                IsNearLadder = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Ladder"))
            {
                IsNearLadder = false;
            }
        }
    }
}
