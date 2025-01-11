using Interfaces;
using UnityEngine;

namespace Mega_man.States
{
    public class ClimbingState : IMovementState
    {
        public void EnterState(IMovementContext movementContext)
        {
            var player = (PlayerMovement)movementContext;
            // Possibly disable gravity while climbing
            player.GravityScale = 0f;
            // Maybe set rigidbody constraints, etc.
        }

        public void ExitState(IMovementContext movementContext)
        {
            var player = (PlayerMovement)movementContext;
            // Re-enable gravity
            player.GravityScale = 1f;
        }

        public void Update(IMovementContext movementContext)
        {
            var player = (PlayerMovement)movementContext;

            // Climbing uses vertical input to move up or down
            Vector2 velocity = player.Rb.linearVelocity;
            velocity.y = player.MovementInput.y * player.Speed;
            velocity.x = 0f; // Keep horizontal velocity zero while climbing
            player.Rb.linearVelocity = velocity;

            // If the player is not pressing up/down or is no longer on a ladder,
            // transition out of climbing
            // e.g. if (NotOnLadder(player)) ...
            if (Mathf.Abs(player.MovementInput.y) < 0.1f)
            {
                // For example, if you let go of the ladder, fallback to in-air
                player.TransitionToState(player.InAirState);
                player.IsNearLadder = false;
            }
        }
    }
}