using Interfaces;
using UnityEngine;

namespace Mega_man.States
{
    public class InAirState : IMovementState
    {
        public void EnterState(IMovementContext movementContext)
        {
            // Maybe you tweak gravity or do an animation trigger
            movementContext.GravityScale = 1f; 
        }

        public void ExitState(IMovementContext movementContext)
        {
        }

        public void Update(IMovementContext movementContext)
        {
            var player = (PlayerMovement)movementContext;

            // 1. Allow horizontal movement in air
            Vector2 velocity = player.Rb.linearVelocity;
            velocity.x = player.MovementInput.x * player.Speed;
            player.Rb.linearVelocity = velocity;

            // 2. Check if landed
            if (IsGrounded(player))
            {
                player.TransitionToState(player.OnGroundState);
            }

            // 3. Potentially handle double jumps, dash in air, etc., if you want
        }

        private bool IsGrounded(PlayerMovement player)
        {
            // The same or a more robust grounded check as in OnGroundState
            return Mathf.Abs(player.Rb.linearVelocity.y) < 0.01f;
        }
    }
}