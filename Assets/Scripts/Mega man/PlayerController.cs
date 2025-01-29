using Events;
using Interfaces;
using Managers;
using Mega_man.States;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mega_man
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour, IMovementContext
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 5f;

        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 10f;         // Initial jump velocity
        [SerializeField] private float jumpBoost = 20f;         // Upward force per second while holding jump
        [SerializeField] private float maxJumpHoldTime = 0.2f;  // How long you can hold jump
        [SerializeField] private float maxUpwardVelocity = 14f; //  velocity clamp
        [SerializeField] private float normalGravityScale = 1f;
        
        [Header("SFX")]
        [SerializeField] private AudioClip landSound;
        
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundCheckRadius = 0.02f;

       

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
        private Animator _animator;
        private Vector2 _lastLadderPosition;
        private HealthManager _healthManager;



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
        public Vector2 LastLadderPosition => _lastLadderPosition;
        public Rigidbody2D Rb => _rb;
        public PlayerAnimationController Anim => _animController;
        public AudioClip LandSound => landSound;
        public Animator Animator => _animator;
        public PlayerShoot Shoot => _playerShooting;


        // Public properties for user input
        public Vector2 MovementInput  { get; set; }
        public bool    JumpPressed    { get; private set; }
        public bool    JumpHeld       { get; private set; }
        public Ladder.Ladder CurrentLadder { get; private set; }
        public bool    IsNearLadder   { get; set; }
        public bool IsFacingRight { get; set; } = true; 
        

        


        private void Awake()
        {
            // Components
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _playerShooting = GetComponent<PlayerShoot>();
            _animController = GetComponent<PlayerAnimationController>();
            _animator = GetComponent<Animator>();
            _healthManager = GetComponent<HealthManager>();
            


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

            _healthManager.OnDie += FreezeCharacter;
            
            


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
            
            _healthManager.OnDie -= FreezeCharacter;


        }

        private void Update()
        {
            // Let the current state handle logic
            _currentState.UpdateState(this);

            // Face direction
            UpdateFacingDirection();

            // Reset JumpPressed so it's one-frame only
            JumpPressed = false;
            
            //print(_rb.linearVelocity);
            print(_currentState);
        }

        private void UpdateFacingDirection()
        {
            float vx = MovementInput.x;
            if (vx < -0.01f)  IsFacingRight = false;
            if (vx >  0.01f)  IsFacingRight = true;

            _spriteRenderer.flipX = !IsFacingRight;
        }

        public void ForceClearVerticalInput()
        {
            MovementInput = new Vector2(MovementInput.x, 0f);
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


        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if we're colliding with something tagged "Ladder"
            if (other.CompareTag("Ladder"))
            {
                IsNearLadder = true;
                // Try to get the Ladder component from this collider or its parent
                Ladder.Ladder ladder = other.GetComponentInParent<Ladder.Ladder>();
                if (ladder != null)
                {
                    CurrentLadder = ladder;
                }
            }
        }

            private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Ladder"))
            {
                IsNearLadder = false;
                // If this exit belongs to the same ladder, clear the reference
                Ladder.Ladder ladder = other.GetComponentInParent<Ladder.Ladder>();
                if (ladder == CurrentLadder)
                {
                    CurrentLadder = null;
                }
            }
        }

        public bool CurrentStateIsInAir()
        {
            return _currentState == _inAirState;
        }

        public bool IsGrounded()
        {
            // 1) Circle center: slightly below the character’s pivot
            Vector2 circleCenter = _rb.position + new Vector2(0f, -0.7f);

            // 2) Circle radius: a small value, like 0.1f
            float circleRadius = 0.2f;
            
            // 4) Perform the Overlap
            Collider2D hit = Physics2D.OverlapCircle(circleCenter, circleRadius, groundLayers);

            // (Optional) Draw a debug line or circle
            Debug.DrawLine(circleCenter, circleCenter + Vector2.down * 0.3f, hit ? Color.green : Color.red);
    
            // 5) Return true if we collided with something in groundLayers
            return (hit != null);
        }


        private void FreezeCharacter()
        {
            //Stop any movement
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;
            enabled = false;
        }

    }
}
