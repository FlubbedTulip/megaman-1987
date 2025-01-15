using Interfaces;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Mega_man.States
{
    public class InAirState : IMovementState
    {
        private float _jumpTimeCounter;    // Tracks how long we've been boosting
        private bool _hasJumpStarted;      // Whether we started the jump in this state

        public void EnterState(IMovementContext context)
        {
            //set animation
            PlayerAnimationManager.SetIsJumping(true);
            
            var player = (PlayerMovement)context;
            
            // We just transitioned into the air. Could be from jumping or falling.
            // If we jumped, JumpPressed would have been set in OnGroundState right before transitioning.
            // If we fell, then we won't do an initial velocity set here.
            
            _jumpTimeCounter = 0f;
            _hasJumpStarted = player.JumpPressed; 
            // If we fell off a ledge, JumpPressed is false. If we jumped, JumpPressed was set to true.

            if (_hasJumpStarted)
            {
                // Apply the initial jump velocity
                Vector2 velocity = player.Rb.linearVelocity;
                velocity.y = player.JumpForce;
                player.Rb.linearVelocity = velocity;
            }
            
            // Normal gravity (or slightly reduced if you like)
            player.GravityScale = player.NormalGravityScale;
        }

        public void ExitState(IMovementContext context)
        {
            // Any cleanup logic when exiting air state.
            PlayerAnimationManager.SetIsJumping(false);

        }

        public void Update(IMovementContext context)
        {
            var player = (PlayerMovement)context;

            // 1. Horizontal Air Control
            Vector2 velocity = player.Rb.linearVelocity;
            velocity.x = player.MovementInput.x * player.Speed;
            player.Rb.linearVelocity = velocity;

            // 2. Variable Jump via Upward Force
            // If we did a jump to get here and still within hold time
            if (_hasJumpStarted && player.JumpHeld && _jumpTimeCounter < player.MaxJumpHoldTime)
            {
                // Each frame, add an upward force to the Rigidbody 
                // ForceMode2D.Force => applying a small, continuous force
                // ForceMode2D.Impulse => applying an instantaneous force, so you might scale by Time.deltaTime
                var upwardForce = player.JumpBoost * Time.deltaTime;
                player.Rb.AddForce(new Vector2(0f, upwardForce), ForceMode2D.Impulse);
                Debug.Log(upwardForce);
                Debug.Log(player.Rb.linearVelocity);

                _jumpTimeCounter += Time.deltaTime;
            }

            // 3. Optional: Clamp Upward Velocity
            // This ensures we don't exceed a certain vertical speed
            if (player.Rb.linearVelocity.y > player.MaxUpwardVelocity)
            {
                player.Rb.linearVelocity = new Vector2(player.Rb.linearVelocity.x, player.MaxUpwardVelocity);
            }

            // 4. Check if we have landed
            if (IsGrounded(player))
            {
                // Transition to OnGroundState
                player.TransitionToState(player.OnGroundState);
            }

            // 5. Optional: Ladder logic mid-air
            // if (player.IsNearLadder && player.MovementInput.y > 0.1f)
            // {
            //     player.TransitionToState(player.ClimbingState);
            //     return;
            // }
        }

        private bool IsGrounded(PlayerMovement player)
        {
            // Use your robust ground check instead of velocity.y
            return Mathf.Abs(player.Rb.linearVelocity.y) < 0.01f;
        }
    }
}
