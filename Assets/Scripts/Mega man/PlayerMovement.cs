using UnityEngine;
using UnityEngine.InputSystem;

namespace Mega_man
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private LayerMask groundLayer;

        private InputActions _inputActions;
        private Vector2 _movement;
        private Rigidbody2D _rb;
        private bool _isGrounded;
        private PlayerState _currentState = PlayerState.Normal;

        private void Awake()
        {
            Debug.Log("Awake called: Initializing Input Actions");
            _inputActions = new InputActions();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            if (_inputActions == null)
            {
                Debug.LogWarning("_inputActions was null. Initializing in OnEnable.");
                _inputActions = new InputActions();
            }

            _inputActions.Player.Enable();
            _inputActions.Player.Jump.performed -= OnJump; // Prevent duplicate bindings
            _inputActions.Player.Jump.performed += OnJump;
        }


        private void OnDisable()
        {
            if (_inputActions != null)
            {
                _inputActions.Player.Jump.performed -= OnJump;
                _inputActions.Player.Disable();
            }
        }

        private void Update()
        {
            // Check if grounded
            _isGrounded = Physics2D.OverlapCircle(transform.position, 0.1f, groundLayer);

            switch (_currentState)
            {
                case PlayerState.Normal:
                    HandleNormalMovement();
                    break;
                case PlayerState.ClimbingLadder:
                    HandleClimbingMovement();
                    break;
            }
        }

        private void HandleNormalMovement()
        {
            _movement = _inputActions.Player.Move.ReadValue<Vector2>();
            _rb.linearVelocity = new Vector2(_movement.x * speed, _rb.linearVelocity.y);
        }

        private void HandleClimbingMovement()
        {
            _movement = _inputActions.Player.Move.ReadValue<Vector2>();
            _rb.linearVelocity = new Vector2(0, _movement.y * speed);
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (_currentState == PlayerState.ClimbingLadder)
            {
                // Drop from ladder and jump
                _currentState = PlayerState.Normal;
                _rb.gravityScale = 1; // Re-enable gravity
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            }
            else if (_currentState == PlayerState.Normal && _isGrounded)
            {
                // Normal jump
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Ladder"))
            {
                _currentState = PlayerState.ClimbingLadder;
                _rb.gravityScale = 0; // Disable gravity
                _rb.linearVelocity = Vector2.zero; // Stop any existing momentum
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Ladder"))
            {
                _currentState = PlayerState.Normal;
                _rb.gravityScale = 1; // Re-enable gravity
            }
        }
    }
    
}
