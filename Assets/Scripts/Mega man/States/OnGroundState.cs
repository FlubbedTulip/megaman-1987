using Interfaces;
using UnityEngine;

namespace Mega_man.States
{
    public class OnGroundState : IMovementState
    {
        public void EnterState(IMovementContext context)
        {
            context.GravityScale = context is PlayerMovement pm
                ? pm.NormalGravityScale
                : 1f;
        }

        public void ExitState(IMovementContext context) { }

        public void Update(IMovementContext context)
        {
            var player = (PlayerMovement)context;

            // Basic horizontal movement
            Vector2 velocity = player.Rb.linearVelocity;
            velocity.x = player.MovementInput.x * player.Speed;
            player.Rb.linearVelocity = velocity;
            PlayerAnimationManager.SetIsRunning(player.Rb.linearVelocity.x != 0);

            // If jump is pressed, apply initial jump force and go to InAirState
            if (player.JumpPressed)
            {
                velocity.y = player.JumpForce;
                player.Rb.linearVelocity = velocity;
                player.TransitionToState(player.InAirState);
                return;
            }

            // Example ground check
            if (!IsGrounded(player))
            {
                player.TransitionToState(player.InAirState);
                return;
            }

            // Check for ladder if needed
            if (player.IsNearLadder && player.MovementInput.y > 0.5f)
            {
                player.TransitionToState(player.ClimbingState);
            }
        }

        private bool IsGrounded(PlayerMovement player)
        {
            // Implement a robust ground check (raycasts, OverlapCircle, etc.).
            return Mathf.Abs(player.Rb.linearVelocity.y) < 0.01f;
        }
    }
}