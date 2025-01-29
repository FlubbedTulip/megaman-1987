using System.Collections;
using Mega_man;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera
{
    public class CameraSwitcher : MonoBehaviour
    {
        [Header("Cameras")]
        public CinemachineCamera topCamera;
        public CinemachineCamera bottomCamera;

        [Header("Transition Settings")]
        public float pauseDuration = 0.5f;      // How long to pause everything initially
        public float transitionDuration = 1f;   // Camera + manual movement duration

        [Header("References")]
        public PlayerInput playerInput;
        [SerializeField] private PlayerController player;
    
        [SerializeField] private Rigidbody2D rb;

        private bool _isSwitching;

        private void Update()
        {
            if (_isSwitching)
            {
                var vector2 = player.Rb.linearVelocity;
                vector2.x = 0;
                player.Rb.linearVelocity = vector2; // Stop any momentum
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !_isSwitching)
            {
                var rb = collision.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    bool isFacingUp = rb.linearVelocity.y > 0f; 
                    StartCoroutine(SwitchCamera(isFacingUp));
                }
            }
        }

        private IEnumerator SwitchCamera(bool isPlayerFacingUp)
        {
            _isSwitching = true;

            // ------------------------------------------------
            // 1) Pause the game fully using timeScale=0
            //    then wait in real-time for pauseDuration
            // ------------------------------------------------
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(pauseDuration);

            // Unpause game
            Time.timeScale = 1f;

            // ------------------------------------------------
            // 2) Disable movement input & set player to Kinematic
            // ------------------------------------------------
            playerInput.actions.Disable();
            player.Rb.bodyType = RigidbodyType2D.Kinematic;
            player.Rb.linearVelocity = Vector2.zero; // Stop any momentum
        

            // ------------------------------------------------
            // 3) Switch camera priorities to trigger Cinemachine blend
            // ------------------------------------------------
            if (isPlayerFacingUp)
            {
                bottomCamera.Priority = 0;
                topCamera.Priority = 1;
            }
            else
            {
                bottomCamera.Priority = 1;
                topCamera.Priority = 0;
            }

            // ------------------------------------------------
            // 4) Manually move the player over transitionDuration
            //    while the camera blend is also happening
            // ------------------------------------------------
            yield return StartCoroutine(HandleManualMove(isPlayerFacingUp));
        
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(pauseDuration);

            // Unpause game
            Time.timeScale = 1f;

            // ------------------------------------------------
            // 5) Re-enable normal physics and input
            // ------------------------------------------------
            player.Rb.bodyType = RigidbodyType2D.Dynamic;
            playerInput.actions.Enable();
        
            if (player.CurrentStateIsInAir())
            {
                var vel = player.Rb.linearVelocity;
                vel.y = Mathf.Min(vel.y, 0); // zero out upward velocity
                vel.y -= 5f;  // push downward
                player.Rb.linearVelocity = vel;
            }

            _isSwitching = false;
            Debug.Log("Camera transition complete. Player movement restored.");
        }

        private IEnumerator HandleManualMove(bool isFacingUp)
        {
            // move the player 1.5 units up or down
            float elapsedTime = 0f;
            Vector3 startPos = player.transform.position;
            Vector3 targetPos = startPos + (isFacingUp ? Vector3.up * 2f : Vector3.down * 2f);

            while (elapsedTime < transitionDuration)
            {
                float t = elapsedTime / transitionDuration;
                player.transform.position = Vector3.Lerp(startPos, targetPos, t);

                elapsedTime += Time.unscaledDeltaTime; 
                yield return null;
            }

            // Snap final
            player.transform.position = targetPos;
        }
    }
}
