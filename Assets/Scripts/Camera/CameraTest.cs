using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    public Transform cameraBorders;
    public float transitionDuration = 1f; // Total duration of the transition
    [SerializeField] private float drag = 50f;
    [SerializeField] private Rigidbody2D playerRb;

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(SwitchCamera());
        }
    }

    private IEnumerator SwitchCamera()
    {
        // Slow down the game
        playerRb.linearDamping = drag; 
        
        cameraBorders.DOMoveY(2, transitionDuration);
        
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

 
        playerRb.linearDamping = 0f; 

    }

}
