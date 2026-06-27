using System;
using NJG.Runtime.Online;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NJG.Runtime.UI
{
    public class LeaderBoardEntryVisual : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _playerName;
        [SerializeField]
        private TextMeshProUGUI _score;
        [SerializeField]
        private Button _raceButton;

        public void Init(LeaderboardEntry entry, Action<LeaderboardEntry> onRaceButtonPressed)
        {
            _playerName.SetText(entry.player_name);
            _score.SetText(entry.score.ToString());
            
            _raceButton.onClick.RemoveAllListeners();
            _raceButton.onClick.AddListener(() => onRaceButtonPressed?.Invoke(entry));
        }
    }
}