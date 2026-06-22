using System;
using NJG.Runtime.Cam;
using NJG.Runtime.Entity;
using NJG.Runtime.Events;
using NJG.Runtime.Input;
using NJG.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NJG.Runtime.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [field: SerializeField]
        public PlayerEntity PlayerPrefab { get; private set; }

        [FoldoutGroup("Scoring"), SerializeField]
        private float _scoreInterval = 1f;
        [FoldoutGroup("Scoring"), SerializeField]
        private int _scorePerInterval = 1;

        private InputProvider _inputProvider;
        private PlayerEntity _player;
        
        private float _scoreTimer;
        private int _score;
        private bool _gameOver;

        public float EnvironmentChunkTriggerZ { get; private set; }

        public IEntity Player => _player;
        public IInputProvider InputProvider => _inputProvider;

        public override void Awake()
        {
            base.Awake();
            
            _inputProvider = new InputProvider();
            _inputProvider.EnablePlayerInput();
        }

        private void Start()
        {
            _player = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
            _player.Init(_inputProvider);
            CameraController.Instance.SetFollowTarget(_player.transform);
            
            _player.OnStoppedMoving += HandleGameOver;
        }

        private void OnDestroy()
        {
            if (_player != null)
                _player.OnStoppedMoving -= HandleGameOver;
            
            _inputProvider.DisablePlayerInput();
        }

        public void Restart()
        {
            // TODO: properly handle level reset.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void HandleGameOver()
        {
            _gameOver = true;
            EventBus.TriggerEvent(new GameOverEvent(_score));
        }

        private void Update()
        {
            if (_gameOver)
                return;
            
            _scoreTimer += Time.deltaTime;
            if (_scoreTimer >= _scoreInterval)
            {
                _scoreTimer -= _scoreInterval;
                _score += _scorePerInterval;
                EventBus.TriggerEvent(new ScoreChangedEvent(_score));
            }
        }
        
        public void UpdateEnvironmentChunkTriggerZ(float z)
        {
            EnvironmentChunkTriggerZ = z;
        }
    }
}