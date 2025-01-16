using Interfaces;
using UnityEngine;

namespace Mega_man.States
{
    public class GroundedState : IMovementState
    {
        public void EnterState(IMovementContext context)
        {
            // Example: set normal gravity
            if (context is PlayerMovement player)
            {
                player.GravityScale = player.NormalGravityScale;
                // Possibly set an animation
                player.Anim.SetRunning(false);
            }
        }

        public void ExitState(IMovementContext context)
        {
            // Any cleanup: e.g., disable "running" animation
        }

        public void UpdateState(IMovementContext context)
        {
            var player = (PlayerMovement)context;

            // 1) Handle horizontal movement
            Vector2 velocity = player.Rb.linearVelocity;
            velocity.x = player.MovementInput.x * player.Speed;
            player.Rb.linearVelocity = velocity;

            // 2) Update Running Animation
            bool isMovingHorizontally = Mathf.Abs(velocity.x) > 0.01f;
            player.Anim.SetRunning(isMovingHorizontally);

            // 3) Check for Jump
            if (player.JumpPressed)
            {
                velocity.y = player.JumpForce;
                player.Rb.linearVelocity = velocity;
                player.TransitionToState(player.InAirState);
                return;
            }

            // 4) If not grounded (e.g. we stepped off a ledge), go in-air
            if (!IsGrounded(player))
            {
                player.TransitionToState(player.InAirState);
                return;
            }

            // 5) Ladder check
            if (player.IsNearLadder && player.MovementInput.y > 0.5f)
            {
                player.TransitionToState(player.ClimbingState);
            }
        }

        private bool IsGrounded(PlayerMovement player)
        {
            // Use a real ground check (raycast, overlap, etc.)
            // For now, we use velocity.y near zero as a placeholder
            return Mathf.Abs(player.Rb.linearVelocity.y) < 0.01f;
        }
    }
}
