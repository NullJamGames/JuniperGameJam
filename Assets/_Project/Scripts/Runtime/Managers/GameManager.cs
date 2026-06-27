using System;
using System.Collections.Generic;
using NJG.Runtime.Cam;
using NJG.Runtime.Entity;
using NJG.Runtime.Events;
using NJG.Runtime.Input;
using NJG.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Managers
{
    public enum GameState
    {
        Playing,
        Paused,
        Menu
    }
    
    public class GameManager : Singleton<GameManager>
    {
        [field: SerializeField]
        public PlayerEntity PlayerPrefab { get; private set; }
        [SerializeField]
        private GhostEntity _ghostEntityPrefab;
        [FoldoutGroup("Scoring"), SerializeField]
        private int _scorePerInterval = 10;
        [FoldoutGroup("Scoring"), SerializeField]
        private int _scorePerCoin = 100;

        private InputProvider _inputProvider;
        private PlayerEntity _player;
        private GhostEntity _ghostEntity;
        
        private int _score;
        private int _highScore;
        private bool _gameOver;

        public float EnvironmentChunkTriggerZ { get; private set; }
        public int PlayerCoins { get; private set; }
        
        private List<Modifier> _shopModifiers = new();
        
        private List<GhostFrame> _queuedGhostFrames;
        private string _ghostName;

        public IEntity Player => _player;
        public IInputProvider InputProvider => _inputProvider;
        
        public GameState GameState { get; private set; } = GameState.Menu;
        public int CurrentLevelSeed { get; private set; }
        public int? QueuedLevelSeed { get; private set; }
        public List<GhostFrame> SavedGhostFrames { get; private set; }
        public List<GhostFrame> LastRunGhostFrames { get; private set; }
        public int CurrentScore => _score;
        public int HighScore => _highScore;

        public override void Awake()
        {
            base.Awake();
            
            CurrentLevelSeed = GenerateLevelSeed();
            
            _inputProvider = new InputProvider();
            _inputProvider.EnablePlayerInput();
        }

        private void OnDestroy()
        {
            if (_player != null)
                _player.OnStoppedMoving -= HandleGameOver;
            
            _inputProvider.DisablePlayerInput();
        }

        [Button(ButtonSizes.Large)]
        public void NewRun()
        {
            if (_player == null)
                InitializePlayer();
            
            _score = 0;
            EventBus.TriggerEvent(new ScoreChangedEvent(_score));

            if (QueuedLevelSeed.HasValue)
            {
                CurrentLevelSeed = QueuedLevelSeed.Value;
                QueuedLevelSeed = null;
            }
            else
            {
                CurrentLevelSeed = GenerateLevelSeed();
            }
            
            EventBus.TriggerEvent(new NewRunEvent(CurrentLevelSeed));
            
            _gameOver = false;
            SetGameState(GameState.Playing);

            foreach (Modifier modifier in _shopModifiers)
            {
                _player.ApplyModifier(modifier);
            }
            
            if (_ghostEntity != null)
                Destroy(_ghostEntity.gameObject);

            if (_queuedGhostFrames != null && _queuedGhostFrames.Count > 0)
            {
                _ghostEntity = Instantiate(_ghostEntityPrefab, Vector3.zero, Quaternion.identity);
                _ghostEntity.Init(new GhostRunData
                {
                    Frames = _queuedGhostFrames,
                    PlayerName = _ghostName
                });
                _queuedGhostFrames = null;
            }
        }

        private void InitializePlayer()
        {
            _player = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
            _player.Init(_inputProvider);
            CameraController.Instance.SetFollowTarget(_player.transform);
            
            _player.OnStoppedMoving += HandleGameOver;
        }

        private void HandleGameOver()
        {
            _gameOver = true;
            SetGameState(GameState.Menu);
            AddPlayerCoins(_score / _scorePerCoin);
            if (_score > _highScore)
                _highScore = _score;
            
            EventBus.TriggerEvent(new GameOverEvent(_score, _highScore));
        }

        private void SetGameState(GameState newState)
        {
            GameState = newState;
            Time.timeScale = newState == GameState.Paused ? 0f : 1f;
        }

        public bool TryPauseGame()
        {
            if (GameState != GameState.Playing)
                return false;
            
            SetGameState(GameState.Paused);
            return true;
        }

        public bool TryResumeGame()
        {
            if (GameState != GameState.Paused)
                return false;

            SetGameState(GameState.Playing);
            return true;
        }

        public void AddScore()
        {
            _score += _scorePerInterval;
            EventBus.TriggerEvent(new ScoreChangedEvent(_score));
        }

        public void AddScore(int amount)
        {
            _score += amount;
            EventBus.TriggerEvent(new ScoreChangedEvent(_score));
        }
        
        public void UpdateEnvironmentChunkTriggerZ(float z)
        {
            EnvironmentChunkTriggerZ = z;
        }

        [Button(ButtonSizes.Large)]
        public void AddPlayerCoins(int coins)
        {
            PlayerCoins += coins;
        }

        public bool TryRemovePlayerCoins(int coins)
        {
            if (PlayerCoins >= coins)
            {
                PlayerCoins -= coins;
                return true;
            }

            return false;
        }

        public void AddShopModifier(BaseModifierSO modifier)
        {
            Modifier modifierInstance = new (modifier, true, 0f, modifier.Color);
            _shopModifiers.Add(modifierInstance);
            _player.ApplyModifier(modifierInstance);
        }
        
        public void QueueGhostRace(List<GhostFrame> frames, int levelSeed, string playerName)
        {
            _queuedGhostFrames = frames;
            _ghostName = $"{playerName}";
            QueuedLevelSeed = levelSeed;
        }

        public void NewGhostFrames(List<GhostFrame> frames)
        {
            LastRunGhostFrames = new List<GhostFrame>(frames);

            if (_score >= _highScore)
                SavedGhostFrames = new List<GhostFrame>(frames);
        }

        public void SetGhostFramesForNextRun(List<GhostFrame> frames)
        {
            SavedGhostFrames = frames;
        }

        public void CopyGhostRunToClipboard()
        {
            if (SavedGhostFrames == null || SavedGhostFrames.Count == 0)
                return;

            GhostRunData runData = new()
            {
                Version = 1,
                PlayerName = "COPIED PLAYER",
                Score = _highScore,
                LevelSeed = CurrentLevelSeed,
                Frames = SavedGhostFrames
            };
            
            string code = GhostRunSerializer.ExportToCode(runData);
            GUIUtility.systemCopyBuffer = code;
        }

        public void ImportGhostRunFromClipboard()
        {
            string code = GUIUtility.systemCopyBuffer;
            
            if (!GhostRunSerializer.TryImportFromCode(code, out GhostRunData runData))
            {
                Debug.LogWarning("Failed to import ghost run from clipboard.");
                return;
            }
            
            SavedGhostFrames = runData.Frames;
            CurrentLevelSeed = runData.LevelSeed;
            
            Debug.Log("Ghost data imported from clipboard");
        }
        
        private int GenerateLevelSeed()
        {
            return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
    }
}