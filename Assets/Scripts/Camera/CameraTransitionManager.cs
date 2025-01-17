using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera
{
    public class CameraSwitcher : MonoBehaviour
    {
        public CinemachineCamera topCamera; // Assign the starting camera in the Inspector
        public CinemachineCamera bottomCamera; // Assign the target camera in the Inspector
        public float transitionDuration = 1f; // Total duration of the transition
        [SerializeField] private PlayerInput playerInput;


        private bool _isSwitching;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !_isSwitching)
            {
                // Check if the player is moving downward relative to the trigger
                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 relativeVelocity = rb.linearVelocity; 
                    bool isFacingUp = relativeVelocity.y > 0;
                    print(isFacingUp);
                    StartCoroutine(SwitchCamera(isFacingUp));
                }
            }
        }

        private IEnumerator SwitchCamera(bool isPlayerFacingUp)
        {
            _isSwitching = true;

            StartCoroutine(PauseTimeCoroutine());

            // Slow down the game
            Time.timeScale = 0.01f;
            playerInput.actions.Disable();
            
            // Switch cameras
            if (isPlayerFacingUp)
            {
                bottomCamera.Priority = 0; // Lower priority
                topCamera.Priority = 1; // Higher priority
            }
            else
            {
                bottomCamera.Priority = 1;
                topCamera.Priority = 0;
            }
            
            Debug.Log($"Starting Transition: {transitionDuration}s");

            float elapsed = 0f;
            while (elapsed <  transitionDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                Debug.Log($"Elapsed: {elapsed}/{transitionDuration}");
                yield return null;
            }
            
            Debug.Log("Transition Complete!");

            
            // Restore normal game speed
            Time.timeScale = 1f;
            playerInput.actions.Enable();
            _isSwitching = false;
            
            Debug.Log("returned to normal time scale");
        }

        
        private IEnumerator PauseTimeCoroutine()
        {
            Time.timeScale = 0f; // Pause the game
            yield return new WaitForSecondsRealtime(0.5f); // Wait without being affected by timeScale
            Time.timeScale = 1f; // Resume the game
        }

       
    }
}
