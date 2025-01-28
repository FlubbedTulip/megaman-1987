using System.Collections;
using Managers;
using TMPro;
using UnityEngine;

namespace EndScreen
{
    public class EndScreenManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI resultText;    // Displays "YOU WIN!" or "YOU LOSE!"
        [SerializeField] private TextMeshProUGUI scoreText;     // Displays the final score
        [SerializeField] private GameObject restartText;        // Flashing "Restart Level" text

        [Header("Settings")]
        [SerializeField] private float flashInterval = 0.5f;    // Time between flashes for "Restart Level"
        private bool _isFlashing;

        private void Start()
        {
            // 1) Display "Win" or "Lose" message
            resultText.text = GameManager.Instance.IsGameWon ? "YOU WIN!" : "YOU LOSE!";

            // 2) Display the player's final score
            scoreText.text = $"Score: {GameManager.Instance.PlayerScore}";

        }

        private void Update()
        {
            // Restart the game (fully) if Enter is pressed
            if (Input.GetKeyDown(KeyCode.Return))
            {
                RestartGame();
            }

            // Quit game if Esc is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
            }
        }

        private IEnumerator FlashRestartText()
        {
            _isFlashing = true;
            while (_isFlashing)
            {
                restartText.SetActive(!restartText.activeSelf);
                yield return new WaitForSeconds(flashInterval);
            }
            // Ensure it's visible after stopping
            restartText.SetActive(true);
        }

        
        private void RestartGame()
        {
            // Instead of reloading the MainLevel scene directly,
            // call GameManager's StartGame() to reset lives, score, etc.
            StartCoroutine(FlashRestartText());

            GameManager.Instance.StartGame();
        }

        private void QuitGame()
        {
            Debug.Log("Quitting Game...");
            Application.Quit();
        }
    }
}
