using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera currentCamera; // Assign the starting camera in the Inspector
    public CinemachineCamera targetCamera; // Assign the target camera in the Inspector
    public float transitionDuration = 1f; // Total duration of the transition
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private PlayerInput playerInput;


    private bool _isSwitching;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_isSwitching)
        {
            StartCoroutine(SwitchCamera());
        }
    }

    private IEnumerator SwitchCamera()
    {
        _isSwitching = true;

        // Slow down the game
        playerRb.linearDamping = 100f; 
        playerInput.enabled = false;
        
        // Switch cameras
        currentCamera.Priority = 10; // Lower priority
        targetCamera.Priority = 11; // Higher priority
        currentCamera.Follow = null;
        targetCamera.Follow = playerTransform;
        
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restore normal game speed
        playerRb.linearDamping = 0f; 
        playerInput.enabled = true;


        // Update the current camera reference
        currentCamera = targetCamera;

        _isSwitching = false;
    }
}
