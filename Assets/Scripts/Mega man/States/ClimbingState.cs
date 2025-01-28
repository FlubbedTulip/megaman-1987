using System;
using Interfaces;
using UnityEngine;

namespace Mega_man.States
{
    public class ClimbingState : IMovementState
    {
        private Collider2D _playerCollider;
        private Collider2D _topEdgeCollider;
        private float _climbingSpeed = 2.5f;

        public void EnterState(IMovementContext context)
        {
            var player = (PlayerMovement)context;
            
            // Disable gravity for climbing
            player.GravityScale = 0f;

        
            _playerCollider = player.GetComponent<Collider2D>();
            if (player.CurrentLadder != null)
            {
                _topEdgeCollider = player.CurrentLadder.edgeCollider;
                if (_playerCollider && _topEdgeCollider)
                {
                    // Ignore only the top edge, so we can climb through
                    Physics2D.IgnoreCollision(_playerCollider, _topEdgeCollider, true);
                }
            }

            // Snap the player's X to the ladder's X if we have a valid ladder
            if (player.CurrentLadder != null)
            {
                float ladderX = player.CurrentLadder.transform.position.x;
                float playerY = player.Rb.position.y;
                if(player.Rb.position.y > player.CurrentLadder.topPosition.position.y)
                {
                    playerY -= 1.5f;
                }
                else playerY += 0.2f;
                Vector2 newPos = new Vector2(ladderX, playerY);
                player.Rb.position = newPos;
            }

            // reset velocity so we start from no Y movement
            player.Rb.linearVelocity = Vector2.zero;

            // Set climbing animation 
            player.Anim.SetClimbing(true);

            Debug.Log("Entered ClimbingState");
        }

        public void ExitState(IMovementContext context)
        {
            var player = (PlayerMovement)context;

            // Re-enable gravity
            player.GravityScale = player.NormalGravityScale;

            // Switch back to normal Player layer
            //player.gameObject.layer = LayerMask.NameToLayer("Default");  

            // Re-enable collision with the top edge
            if (_playerCollider && _topEdgeCollider)
            {
                Physics2D.IgnoreCollision(_playerCollider, _topEdgeCollider, false);
            }

            // Stop climbing animation
             player.Anim.SetClimbing(false);
        }

        public void UpdateState(IMovementContext context)
        {
            var player = (PlayerMovement)context;

            // If there's no ladder reference, or the player left the ladder area, fall
            if (player.CurrentLadder == null || !player.IsNearLadder)
            {
                player.TransitionToState(player.InAirState);
                return;
            }

            // Climb using vertical input
            Vector2 velocity = player.Rb.linearVelocity;
            velocity.x = 0f; // Lock horizontal while climbing
            velocity.y = player.MovementInput.y * _climbingSpeed;
            player.Rb.linearVelocity = velocity;
            
            //Set animator speed based on movement
            if (Mathf.Abs(player.MovementInput.y) < 0.01f 
                && player.Rb.bodyType != RigidbodyType2D.Kinematic) // only freeze if you're truly not moving & not in manual kinematic mode
            {
                player.Animator.speed = 0f;
            }
            else
            {
                player.Animator.speed = 1f;
            }

            // -- DETECT REACHING TOP OR BOTTOM --
            CheckAndHandleLadderEdges(player);

            // If the player presses jump, let's make them just fall
            if (player.JumpPressed)
            {
                // Switch to in-air: no actual jump impulse, just gravity
                player.TransitionToState(player.InAirState);
            }
        }

        private void CheckAndHandleLadderEdges(PlayerMovement player)
        {
            Ladder.Ladder ladder = player.CurrentLadder;
            if (ladder == null) return;

            // Current Y position
            float playerY = player.Rb.position.y;
            float topY = ladder.topPosition.position.y;
            float bottomY = ladder.bottomPosition.position.y;

            // If we've reached the top
            if (playerY >= topY)
            {
                // Snap to top exit
                if (ladder.topExitPosition != null)
                {
                    player.Rb.position = ladder.topExitPosition.position;
                }
                // Force clear vertical input
                player.ForceClearVerticalInput();
                // Transition to grounded after snapping
                player.TransitionToState(player.GroundedState);
            }
            // If we've reached the bottom
            else if (playerY <= bottomY)
            {
                Debug.Log("reached bottom");
                // Snap to bottom exit
                if (ladder.bottomExitPosition != null)
                {
                    player.Rb.position = ladder.bottomExitPosition.position;
                }
                // Force clear vertical input
                player.ForceClearVerticalInput();
                // Transition to grounded after snapping
                player.TransitionToState(player.GroundedState);
            }
        }

    
    }
}
