using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraTransition : MonoBehaviour
{
    public CinemachineCamera virtualCamera;
    public float transitionDuration = 2f;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            print("TESTTT");
            StartCoroutine(HandleTransition(collision.transform));
        }
    }

    private IEnumerator HandleTransition(Transform player)
    {
        isTransitioning = true;

        // Freeze game
        Time.timeScale = 0;
        
        // Temporarily disable Follow
        Transform originalFollowTarget = virtualCamera.Follow;
        virtualCamera.Follow = null;

        // Determine new camera position
        Vector3 newCameraPosition = virtualCamera.transform.position;

        // if (CompareTag("Top Border trigger"))
        // {
        //     newCameraPosition.y += virtualCamera.Lens.OrthographicSize * 2;
        //     player.position = new Vector2(player.position.x, player.position.y - virtualCamera.Lens.OrthographicSize * 2);
        // }
        if (CompareTag("Bottom Border Trigger"))
        {
            print("tes");
            newCameraPosition.y -= virtualCamera.Lens.OrthographicSize * 2;
        }

        // Smoothly move the camera
        float elapsedTime = 0f;
        Vector3 startingPosition = virtualCamera.transform.position;

        while (elapsedTime < transitionDuration)
        {
            virtualCamera.transform.position = Vector3.Lerp(startingPosition, newCameraPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        virtualCamera.transform.position = newCameraPosition;

        // Force Cinemachine to update to the new camera position
        CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        cinemachineBrain.ManualUpdate();

        // Unfreeze game
        Time.timeScale = 1;

        // Re-enable Follow
        virtualCamera.Follow = originalFollowTarget;
        
        isTransitioning = false;
    }
    
}