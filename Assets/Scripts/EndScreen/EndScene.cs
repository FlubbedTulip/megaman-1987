using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
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
            //scoreText.text = $"Final Score: {GameManager.Instance.PlayerScore}";

            // 3) Start flashing the restart text
            StartCoroutine(FlashRestartText());
        }

        private void Update()
        {
            // Restart the game (fully) if Enter is pressed
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopFlashing();
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

        private void StopFlashing()
        {
            _isFlashing = false;
        }

        private void RestartGame()
        {
            // Instead of reloading the MainLevel scene directly,
            // call GameManager's StartGame() to reset lives, score, etc.
            GameManager.Instance.StartGame();
        }

        private void QuitGame()
        {
            Debug.Log("Quitting Game...");
            Application.Quit();
        }
    }
}
