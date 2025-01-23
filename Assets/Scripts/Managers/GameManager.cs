using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private bool _isGameOver;
        private bool _playerReady;
        private bool _initReadyScreen;
        
        private int _playerScore;

        private float _gameRestartTime;
        private float _gamePlayerReadyTime;
        
        public float gameRestartDelay = 5f;
        public float gamePlayerReadyDelay = 3f;
        
        private TextMeshProUGUI _scoreText;
        private TextMeshProUGUI _screenMessageText;


        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartGame();
        }

        private void StartGame()
        {
            _isGameOver = false;
            _playerReady = true;
            _initReadyScreen = true;
            _gamePlayerReadyTime = gamePlayerReadyDelay;
            
            _scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
            _screenMessageText = GameObject.Find("ScreenMessage").GetComponent<TextMeshProUGUI>();
            
            SoundManager.Instance.musicSource.Play();
        }
    }
}
