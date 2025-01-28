using Events;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private string startSceneName = "StartMenu";
        [SerializeField] private string mainSceneName  = "MainScene";
        [SerializeField] private string endSceneName   = "EndScene";

        [Header("Music Clips")]
        [SerializeField] private AudioClip mainLevelMusic; // Music for the MainLevel

        [Header("Gameplay Settings")]
        [SerializeField] private int maxLives = 3;                // Total lives
        [SerializeField] private float readyDelay = 3f;           // Seconds to show “READY” before spawning Mega Man
        
        [Header("Player prefab")]
        private GameObject _playerObject;

        // Internal references
        private TextMeshProUGUI _screenMessage;
        private TextMeshProUGUI _score;
        private bool _showingReady;
        private float _readyTimer;

        // Lives & state
        private int _currentLives;
        public bool IsGameWon { get; private set; }   // Used by End Scene to display Win/Lose

        private bool _isPlayerSpawned;
        private bool _isGameOver;
        

        public int PlayerScore {get; private set;}
        
        
        private static GameManager _instance;
        public static GameManager Instance => _instance;
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            if (SceneManager.GetActiveScene().name == mainSceneName)
            {
                SetupMainLevel();
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            GameEvents.PlayerDeath += PlayerDied;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GameEvents.PlayerDeath -= PlayerDied;
        }

        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == mainSceneName)
            {
                //SetupMainLevel();
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
                    _screenMessage.text = "READY";
                }

                if (_readyTimer <= 0f)
                {
                    _showingReady = false;
                    if (_screenMessage != null) _screenMessage.text = "";

                    // TODO: Spawn or enable Mega Man here
                    _playerObject.SetActive(true);
                    _isPlayerSpawned = true;
                }
            }
        }

        public void StartGame()
        {
            // Reset lives each time we start the game fresh
            _currentLives = maxLives;
            PlayerScore  = 0;          // reset score
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
            SoundManager.Instance.PlayMusic(mainLevelMusic);
            
            // Start “READY” countdown
            _readyTimer = readyDelay;
            _showingReady = true;
            _screenMessage.text = "READY";
        }

        // ———————————————————————————————————————————
        // LIVES / DEATH LOGIC
        // ———————————————————————————————————————————

        /// <summary>
        /// Called when Mega Man dies once.
        /// </summary>
        private void PlayerDied()
        {
            if (_isGameOver) return;
            print(_currentLives);
            _currentLives--;
            print(_currentLives);
            Debug.Log($"Player died. Lives left: {_currentLives}");

            if (_currentLives > 0)
            {
                SceneManager.LoadScene(mainSceneName);
            }
            else
            {
                _isGameOver = true;
                IsGameWon = false;
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
            PlayerScore += points;
            UpdateScoreUI();
        }
        
        private void UpdateScoreUI()
        {
            if (_score != null)
            {
                // Format the score as 7 digits, padded with zeros.
                // e.g. 42 -> "0000042"
                _score.text = PlayerScore.ToString("D7"); 
            }
        }


        // ———————————————————————————————————————————
        // END SCENE
        // ———————————————————————————————————————————

        private void GoToEndScene()
        {
            SceneManager.LoadScene(endSceneName);
        }

        public void SetMainSceneReferences(TextMeshProUGUI scoreText, TextMeshProUGUI screenMessage, GameObject megaManPrefab)
        {
            _score = scoreText;
            _screenMessage = screenMessage;
            _playerObject = megaManPrefab;
            
            SetupMainLevel();
        }
    }
}
