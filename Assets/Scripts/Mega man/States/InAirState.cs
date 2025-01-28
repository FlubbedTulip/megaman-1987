using System;
using Interfaces;
using Managers;
using UnityEngine;

namespace Mega_man.States
{
    public class InAirState : IMovementState
{
    private float _jumpTimeCounter;   // How long we've been applying jump-boost
    private bool _hasJumpStarted;     // Did we actually do a jump, or are we just falling?
    private bool _isFallingFromLadder; // New flag to handle falling from a ladder
    

    public void EnterState(IMovementContext context)
    {
        var player = (PlayerMovement)context;

        // Check if we are falling from a ladder
        _isFallingFromLadder = !player.JumpPressed && !IsGrounded(player);

        // Set jump animation
        player.Anim.SetJumping(true);

        // Reset counters
        _jumpTimeCounter = 0f;
        _hasJumpStarted = !_isFallingFromLadder && player.JumpPressed;

        // If we jumped from the ground, apply initial velocity
        if (_hasJumpStarted)
        {
            Debug.Log("Jump started");
            Vector2 velocity = player.Rb.linearVelocity;
            velocity.y = player.JumpForce;
            player.Rb.linearVelocity = velocity;
        }

        // Normal gravity
        player.GravityScale = player.NormalGravityScale;
    }

    public void ExitState(IMovementContext context)
    {
        var player = (PlayerMovement)context;

        // Turn off jump animation when we leave the air
        player.Anim.SetJumping(false);
    }

    public void UpdateState(IMovementContext context)
    {
        var player = (PlayerMovement)context;

        // 1) Horizontal Air Control
        Vector2 velocity = player.Rb.linearVelocity;
        velocity.x = player.MovementInput.x * player.Speed;
        player.Rb.linearVelocity = velocity;

        // 2) Variable Jump
        if (!_isFallingFromLadder && _hasJumpStarted && player.JumpHeld && _jumpTimeCounter < player.MaxJumpHoldTime)
        {
            float upwardForce = player.JumpBoost * Time.deltaTime;
            player.Rb.AddForce(new Vector2(0f, upwardForce), ForceMode2D.Impulse);
            _jumpTimeCounter += Time.deltaTime;
        }

        // 3) Clamp Upward Velocity
        if (player.Rb.linearVelocity.y > player.MaxUpwardVelocity)
        {
            player.Rb.linearVelocity = new Vector2(
                player.Rb.linearVelocity.x,
                player.MaxUpwardVelocity
            );
        }

        // 4) Check if grounded (landed)
        if (IsGrounded(player))
        {
            // Play the landing SFX
            SoundManager.Instance.PlaySound(player.LandSound);
            player.TransitionToState(player.GroundedState);
        }

        //5) Ladder check if you can grab a ladder mid-air
        if (player.IsNearLadder && player.MovementInput.y > 0f)
        {
            player.TransitionToState(player.ClimbingState);
        }
    }

    private bool IsGrounded(PlayerMovement player)
    {
        // Raycast approach instead of velocity check
        Vector2 origin = player.Rb.position;
        origin.y -= 0.7f;
        float distance = 0.2f; // or read from a serialized field
        LayerMask layers = player.groundLayers; // if you store it in PlayerMovement

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, distance, layers);
        Debug.DrawLine(origin, origin + Vector2.down * distance, Color.yellow);

        return (hit.collider != null);
    }

}

}
