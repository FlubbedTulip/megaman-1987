using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [Header("Scene Names")]
        public string startSceneName = "StartMenu";
        public string mainSceneName  = "MainScene";
        public string endSceneName   = "EndScene";

        [Header("Music Clips")]
        public AudioClip mainLevelMusic; // Music for the MainLevel

        [Header("Gameplay Settings")]
        public int maxLives = 3;                // Total lives
        public float readyDelay = 3f;           // Seconds to show “READY” before spawning Mega Man

        // Internal references
        private TextMeshProUGUI _screenMessage;
        private bool _showingReady;
        private float _readyTimer;

        // Lives & state
        private int _currentLives;
        public bool IsGameWon { get; private set; }   // Used by End Scene to display Win/Lose

        private bool _isPlayerSpawned;
        private bool _isGameOver;

        private int _playerScore { get; set;}

        private void Start()
        {
            // Optional if you want the GameManager to persist across scenes:
            // DontDestroyOnLoad(gameObject);

            // Initialize lives
            _currentLives = maxLives;

            // If we start in the Start scene, do nothing special here,
            // If we start directly in MainLevel for testing, we can do SetupMainLevel.
            if (SceneManager.GetActiveScene().name == mainSceneName)
            {
                print("test");
                SetupMainLevel();
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Called automatically whenever a new scene is loaded
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == mainSceneName)
            {
                SetupMainLevel();
            }
            else if (scene.name == endSceneName)
            {
                // The EndScene will check IsGameWon to display “You Win” or “You Lose.”
                // Any end-scene music or logic can go here if you wish.
            }
            // If it's the Start scene, we typically do nothing until the user starts the game.
        }

        private void Update()
        {
            // Handle "READY" countdown in MainLevel
            if (_showingReady)
            {
                _readyTimer -= Time.deltaTime;
                if (_screenMessage != null)
                {
                    _screenMessage.text = $"READY\n{Mathf.CeilToInt(_readyTimer)}";
                }

                if (_readyTimer <= 0f)
                {
                    _showingReady = false;
                    if (_screenMessage != null) _screenMessage.text = "";

                    // TODO: Spawn or enable Mega Man here
                    // e.g. Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                    // or playerObject.SetActive(true);
                    _isPlayerSpawned = true;
                    Debug.Log("Mega Man spawned!");
                }
            }
        }

        /// <summary>
        /// Called from e.g. a “Play” button in the Start scene
        /// </summary>
        public void StartGame()
        {
            // Reset lives each time we start the game fresh
            _currentLives = maxLives;
            _playerScore  = 0;          // reset score
            _isGameOver = false;
            SceneManager.LoadScene(mainSceneName);
        }

        /// <summary>
        /// Set up MainLevel: play music, show “READY”, etc.
        /// </summary>
        private void SetupMainLevel()
        {
            _isGameOver = false;
            _isPlayerSpawned = false;
            IsGameWon = false; // Reset previous state if any

            // Play the main level music
            SoundManager.Instance.PlayMusic(mainLevelMusic, true);

            // Attempt to find a “ScreenMessage” UI text for the READY message
            _screenMessage = GameObject.Find("ScreenMessage")?.GetComponent<TextMeshProUGUI>();
            if (_screenMessage != null)
            {
                // Start “READY” countdown
                _readyTimer = readyDelay;
                _showingReady = true;
                _screenMessage.text = $"READY\n{Mathf.CeilToInt(_readyTimer)}";
            }
        }

        // ———————————————————————————————————————————
        // LIVES / DEATH LOGIC
        // ———————————————————————————————————————————

        /// <summary>
        /// Called when Mega Man dies once.
        /// </summary>
        public void PlayerDied()
        {
            if (_isGameOver) return;

            // Decrement lives
            _currentLives--;
            Debug.Log($"Player died. Lives left: {_currentLives}");

            if (_currentLives > 0)
            {
                // Restart the main scene for another attempt
                SceneManager.LoadScene(mainSceneName);
            }
            else
            {
                // Out of lives → game over
                _isGameOver = true;
                IsGameWon = false; // We lost
                GoToEndScene();
            }
        }

        /// <summary>
        /// Called when the boss is defeated or the level is completed.
        /// </summary>
        public void BossDefeated()
        {
            if (_isGameOver) return;
            _isGameOver = true;
            IsGameWon = true; // We won
            GoToEndScene();
        }


        public void AddScore(int points)
        {
            _playerScore += points;
            // Possibly call UpdateScoreUI();
        }


        // ———————————————————————————————————————————
        // END SCENE
        // ———————————————————————————————————————————

        private void GoToEndScene()
        {
            SceneManager.LoadScene(endSceneName);
        }
    }
}
