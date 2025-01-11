using Interfaces;
using UnityEngine;

namespace Mega_man.States
{
    public class OnGroundState : IMovementState
    {
        public void EnterState(IMovementContext movementContext)
        {
            // Example: ensure gravity is on
            movementContext.GravityScale = 1f;
        }

        public void ExitState(IMovementContext movementContext)
        {
            // Called once before leaving OnGroundState
        }

        public void Update(IMovementContext movementContext)
        {
            // Cast the context to PlayerMovement if you need access to specific data
            var player = (PlayerMovement)movementContext;

            // 1. Move horizontally
            Vector2 velocity = player.Rb.linearVelocity;
            velocity.x = player.MovementInput.x * player.Speed;
            player.Rb.linearVelocity = velocity;

            // 2. Check for jump
            if (player.JumpPressed)
            {
                // Apply jump force
                player.Rb.linearVelocity = new Vector2(player.Rb.linearVelocity.x, player.JumpForce);

                // Transition to in-air
                player.TransitionToState(player.InAirState);
                return;
            }

            // 3. Check if we fell off an edge
            //    E.g., if velocity.y < 0.1f or a grounded check fails
            //    For a real game you'd likely use raycasts or colliders to check if 
            //    the player is still grounded. Here is just an example:
            if (!IsGrounded(player))
            {
                player.TransitionToState(player.InAirState);
                return;
            }

            // 4. If near a ladder and pressing up => transition to climbing
            //    For example:
            //    if (IsNearLadder(player) && player.MovementInput.y > 0.1f)
            //    {
            //        player.TransitionToState(player.ClimbingState);
            //    }
        }

        private bool IsGrounded(PlayerMovement player)
        {
            // TODO: your grounded check (e.g., raycast, collision check, etc.)
            return player.Rb.linearVelocity.y == 0f; // naive example
        }
    }
}
