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
        [SerializeField] private AudioClip bossMusic;

        [Header("Gameplay Settings")]
        [SerializeField] private int maxLives = 3;                // Total lives
        [SerializeField] private float readyDelay = 3f;      // Seconds to show “READY” before spawning Mega Man
        

        // Internal references
        private TextMeshProUGUI _screenMessage;
        private TextMeshProUGUI _score;
        private bool _showingReady;
        private float _readyTimer;
        private GameObject _playerObject;
        private GameObject _bossObject;
        private GameObject _bossUIHealth;

        // Lives & state
        private int _currentLives;
        public bool IsGameWon { get; private set; }   // Used by End Scene to display Win/Lose

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
            GameEvents.BossStart += StartBossFight;
            GameEvents.BossDeath += BossDefeated;
        }
        

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GameEvents.PlayerDeath -= PlayerDied;
            GameEvents.BossStart -= StartBossFight;
            GameEvents.BossDeath -= BossDefeated;
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
        
        
        // Set up MainLevel: play music, show “READY”, etc.
        private void SetupMainLevel()
        {
            _isGameOver = false;
            IsGameWon = false; // Reset previous state if any

            // Play the main level music
            SoundManager.Instance.PlayMusic(mainLevelMusic);
            
            // Start “READY” countdown
            _readyTimer = readyDelay;
            _showingReady = true;
            _screenMessage.text = "READY";
            UpdateScoreUI();
        }



        
        //Called when Mega Man dies once.
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
        
        
        private void StartBossFight()
        {
            SoundManager.Instance.StopMusic();
            SoundManager.Instance.PlayMusic(bossMusic);
            _bossObject.SetActive(true);
            _bossUIHealth.SetActive(true);
        }
        
        private void BossDefeated()
        {
            if (_isGameOver) return;
            _isGameOver = true;
            IsGameWon = true; 
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
                _score.text = PlayerScore.ToString("D7"); 
            }
        }



        private void GoToEndScene()
        {
            SceneManager.LoadScene(endSceneName);
        }

        public void SetMainSceneReferences(TextMeshProUGUI scoreText, TextMeshProUGUI screenMessage, GameObject megaManPrefab, GameObject bossPrefab, GameObject bossUIHealthPrefab)
        {
            _score = scoreText;
            _screenMessage = screenMessage;
            _playerObject = megaManPrefab;
            _bossObject = bossPrefab;
            _bossUIHealth = bossUIHealthPrefab;
            SetupMainLevel();
        }
    }
}
