using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject blackSprite; // The black sprite covering the text
    [SerializeField] private string mainLevelSceneName = "MainScene"; // Name of the main level scene
    [SerializeField] private float flashInterval = 0.5f; // Time between flashes
    [SerializeField] private float transitionDelay = 2f; // Delay after pressing Enter before transitioning
    [SerializeField] private AudioClip levelSong; 
    [SerializeField] private AudioClip levelStart; 


    private bool _hasPressedStart = false; // Tracks if the player has already pressed start

    private void Start()
    {
        // Start the flashing coroutine
        blackSprite.SetActive(false); // Ensure the text is visible
        SoundManager.Instance.PlayMusic(levelSong);
    }

    private void Update()
    {
        // Check for Enter key press
        if (Input.GetKeyDown(KeyCode.Return) && !_hasPressedStart)
        {
            _hasPressedStart = true; // Prevent multiple presses
            SoundManager.Instance.PlayMusic(levelStart);
            StartCoroutine(FlashText()); 
            StartCoroutine(TransitionToMainLevel()); // Start the transition
        }
    }

    private IEnumerator FlashText()
    {
        while (true)
        {
            // Toggle the black sprite
            blackSprite.SetActive(!blackSprite.activeSelf);
            yield return new WaitForSeconds(flashInterval); // Wait for the flash interval
        }
    }

    private IEnumerator TransitionToMainLevel()
    {
        yield return new WaitForSeconds(transitionDelay); // Wait before transitioning
        SceneManager.LoadScene(mainLevelSceneName); // Load the main level scene
    }
}
