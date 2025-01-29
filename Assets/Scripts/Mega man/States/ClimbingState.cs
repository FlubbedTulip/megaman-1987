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
        
        private float _shootFreezeTime;
        private const float SHOOT_FREEZE_DURATION = 0.2f;

        public void EnterState(IMovementContext context)
        {
            var player = (PlayerController)context;
            
            player.Shoot.OnShoot += OnShootWhileClimbing;
            
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
            var player = (PlayerController)context;

            // Re-enable gravity
            player.GravityScale = player.NormalGravityScale;
            
            player.Shoot.OnShoot -= OnShootWhileClimbing;
            
            player.Animator.speed = 1f;
            
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
            var player = (PlayerController)context;

            // 1) If there's no ladder reference, or the player left the ladder area, fall
            if (player.CurrentLadder == null || !player.IsNearLadder)
            {
                player.TransitionToState(player.InAirState);
                return;
            }

            // 2) Prepare velocity (always lock horizontal while on ladder)
            Vector2 velocity = player.Rb.linearVelocity;
            velocity.x = 0f;

            // 3) If we are “shoot freezing,” override movement & animator speed
            if (_shootFreezeTime > 0f)
            {
                // Decrease the lock timer & block vertical movement
                _shootFreezeTime -= Time.deltaTime;
                velocity.y = 0f; // can't move up/down

                // Keep animation playing (no freeze) so it doesn't get stuck
                player.Animator.speed = 1f;
            }
            else
            {
                // Normal climb speed
                velocity.y = player.MovementInput.y * _climbingSpeed;

                // -- Freeze or not freeze the animation based on vertical input
                if (Mathf.Abs(player.MovementInput.y) < 0.01f 
                    && player.Rb.bodyType != RigidbodyType2D.Kinematic)
                {
                    // If not moving, freeze anim
                    player.Animator.speed = 0f;
                }
                else
                {
                    // If moving, animate at speed=1
                    player.Animator.speed = 1f;
                }
            }

            player.Rb.linearVelocity = velocity;

            // 4) Detect top/bottom of ladder
            CheckAndHandleLadderEdges(player);

            // 5) If the player presses jump, switch to in-air
            if (player.JumpPressed)
            {
                player.TransitionToState(player.InAirState);
            }
        }


        private void CheckAndHandleLadderEdges(PlayerController player)
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
        
        
        private void OnShootWhileClimbing()
        {
            // Lock vertical movement for a short time
            _shootFreezeTime = SHOOT_FREEZE_DURATION;
        }

    
    }
}
