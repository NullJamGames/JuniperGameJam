using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NJG.Runtime.Entity;
using NJG.Runtime.Managers;
using NJG.Runtime.Online;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace NJG.Runtime.UI
{
    public class LeaderboardPanel : MenuPanel
    {
        [FoldoutGroup("Dependencies"), SerializeField]
        private TextMeshProUGUI _gameOverScoreText;
        [FoldoutGroup("Dependencies"), SerializeField]
        private TextMeshProUGUI _privacyDisclosureText;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Button _submitHighScoreButton;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Button _mainMenuButton;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Button _upgradeShopButton;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Button _tryAgainButton;
        [FoldoutGroup("Dependencies"), SerializeField]
        private LeaderBoardEntryVisual _leaderBoardEntryPrefab;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Transform _leaderBoardEntryContainer;
        [FoldoutGroup("Dependencies"), SerializeField]
        private TMP_InputField _playerNameInput;
        
        [FoldoutGroup("Leaderboard"), SerializeField]
        private string _defaultPlayerName = "Player";
        
        public void Initialize(Action onTryAgain, Action onUpgradeShop, Action onMainMenu)
        {
            _tryAgainButton.onClick.AddListener(() => onTryAgain?.Invoke());
            _upgradeShopButton.onClick.AddListener(() => onUpgradeShop?.Invoke());
            _mainMenuButton.onClick.AddListener(() => onMainMenu?.Invoke());
            _submitHighScoreButton.onClick.AddListener(OnButton_SubmitHighScore);
            
            _privacyDisclosureText.SetText(
                "Submitting uploads your chosen name, score, and ghost replay data for leaderboard and ghost racing.");
        }

        public override void OnShow()
        {
            base.OnShow();
            
            RefreshLeaderboard();
            SetSubmitButtonInteractable(false);
        }
        
        public override void OnHide()
        {
            base.OnHide();
            
            _gameOverScoreText.SetText(string.Empty);
        }

        public void ShowGameOverText(int score, int highScore)
        {
            SetSubmitButtonInteractable(true);
            _gameOverScoreText.SetText(score == highScore
                ? $"NEW HIGH SCORE!\n<color=green>{score}</color>"
                : $"Your score this run was <color=yellow>{score}</color>.\nYour high score is <color=green>{highScore}</color>.");
        }

        private void SetSubmitButtonInteractable(bool interactable)
        {
            _submitHighScoreButton.interactable = interactable;
        }

        private void ShowLeaderboard(IReadOnlyList<LeaderboardEntry> entries, Action<LeaderboardEntry> onRacePressed)
        {
            foreach (Transform child in _leaderBoardEntryContainer)
                Destroy(child.gameObject);

            foreach (LeaderboardEntry entry in entries)
            {
                LeaderBoardEntryVisual visual = Instantiate(_leaderBoardEntryPrefab, _leaderBoardEntryContainer);
                visual.Init(entry, onRacePressed);
            }
        }
        
        private void OnButton_SubmitHighScore()
        {
            List<GhostFrame> frames = GameManager.Instance.LastRunGhostFrames;

            if (frames == null || frames.Count == 0)
            {
                Debug.LogWarning("No ghost frames to submit.");
                return;
            }

            string playerName = GetValidatedPlayerName();

            GhostRunData runData = new()
            {
                Version = 1,
                PlayerName = playerName,
                Score = GameManager.Instance.CurrentScore,
                LevelSeed = GameManager.Instance.CurrentLevelSeed,
                Frames = frames
            };

            string ghostData = GhostRunSerializer.ExportToCode(runData);

            SetSubmitButtonInteractable(false);
            
            LeaderboardClient.Instance.SubmitRun(
                playerName + Random.Range(1,9999),
                GameManager.Instance.CurrentScore,
                ghostData,
                GameManager.Instance.CurrentLevelSeed,
                success =>
                {
                    if (success)
                        RefreshLeaderboard();
                });
        }
        
        private void RefreshLeaderboard()
        {
            LeaderboardClient.Instance.GetTop20(entries =>
            {
                ShowLeaderboard(entries, OnRaceLeaderboardEntry);
            });
        }
        
        private void OnRaceLeaderboardEntry(LeaderboardEntry entry)
        {
            LeaderboardClient.Instance.GetGhostData(entry.id, ghostData =>
            {
                if (string.IsNullOrWhiteSpace(ghostData))
                    return;

                if (!GhostRunSerializer.TryImportFromCode(ghostData, out GhostRunData runData))
                {
                    Debug.LogWarning("Failed to import leaderboard ghost.");
                    return;
                }

                GameManager.Instance.QueueGhostRace(runData.Frames, runData.LevelSeed, runData.PlayerName);
                GameManager.Instance.NewRun();
            });
        }
        
        private string GetValidatedPlayerName()
        {
            string playerName = _playerNameInput.text.Trim();

            // Empty name
            if (string.IsNullOrWhiteSpace(playerName))
                return _defaultPlayerName;

            // Limit length
            if (playerName.Length > 20)
                playerName = playerName[..20];

            // Too short
            if (playerName.Length < 2)
                return "ShortName";

            // Normalize for filtering
            string normalized = NormalizeName(playerName);

            // Check against banned words
            if (GetBannedWords().Any(word => normalized.Contains(word)))
                return "BadName";

            return playerName;
        }

        private static string NormalizeName(string name)
        {
            name = name.ToLowerInvariant();

            // Common leetspeak replacements
            name = name
                .Replace('0', 'o')
                .Replace('1', 'i')
                .Replace('3', 'e')
                .Replace('4', 'a')
                .Replace('5', 's')
                .Replace('7', 't')
                .Replace('@', 'a')
                .Replace('$', 's');

            // Remove spaces, punctuation, underscores, etc.
            name = Regex.Replace(name, @"[\W_]", "");

            return name;
        }

        private static HashSet<string> GetBannedWords()
        {
            return new HashSet<string>
            {
                // General profanity
                "fuck",
                "fucking",
                "fucker",
                "motherfucker",
                "shit",
                "bullshit",
                "asshole",
                "bitch",
                "bastard",
                "damn",
                "dick",
                "penis",
                "vagina",
                "cock",
                "cunt",
                "pussy",
                "whore",
                "slut",
                "tit",
                "tits",
                "boob",
                "boobs",
                "porn",
                "sex",
                "anal",
                "blowjob",
                "cum",
                "cumming",
                "semen",

                // Common insults / toxic names
                "idiot",
                "moron",
                "retard",
                "stupid",
                "loser",

                // Self-harm / violence references
                "suicide",
                "killyourself",
                "kys",

                // Nazi / extremist references
                "hitler",
                "nazi",
                "ss",
                "heilhitler",

                // Racist slurs (include only the normalized roots)
                // Intentionally abbreviated here; expand as needed for your community.
                "nigger",
                "nigga",
                "chink",
                "spic",
                "kike",
                "faggot",
                "fag"
            };
        }
    }
}