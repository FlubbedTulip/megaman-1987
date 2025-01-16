using Interfaces;
using UnityEngine;

namespace Mega_man.States
{
    public class ClimbingState : IMovementState
    {
        public void EnterState(IMovementContext context)
        {
            var player = (PlayerMovement)context;
            // Disable gravity while climbing
            player.GravityScale = 0f;
            // set a climbing animation
            //PlayerAnimationManager.SetIsClimbing(true);
        }

        public void ExitState(IMovementContext context)
        {
            var player = (PlayerMovement)context;
            // Re-enable gravity
            player.GravityScale = player.NormalGravityScale;
            // Turn off climbing animation
            //PlayerAnimationManager.SetIsClimbing(false);
        }

        public void UpdateState(IMovementContext context)
        {
            var player = (PlayerMovement)context;

            // Climb using vertical input
            Vector2 velocity = player.Rb.linearVelocity;
            velocity.y = player.MovementInput.y * player.Speed;
            velocity.x = 0f; 
            player.Rb.linearVelocity = velocity;

            // If player stops pressing up/down or is no longer near ladder, exit
            if (Mathf.Abs(player.MovementInput.y) < 0.1f || !player.IsNearLadder)
            {
                // If we just let go, we probably fall => InAirState
                player.TransitionToState(player.InAirState);
                player.IsNearLadder = false;
            }
            
            // If we press jump on the ladder, we might want to jump off:
            if (player.JumpPressed)
            {
                player.TransitionToState(player.InAirState);
            }
        }
    }
}