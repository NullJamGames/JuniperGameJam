using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class GhostEntity : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _playerNameText;
        
        private IReadOnlyList<GhostFrame> _ghostFrames;
        private int _index;
        public string PlayerName { get; private set; }

        public void Init(GhostRunData runData)
        {
            _ghostFrames = runData.Frames;
            _index = 0;
            PlayerName = runData.PlayerName;
            _playerNameText.text = PlayerName;
        }

        private void Update()
        {
            _playerNameText.transform.parent.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        }

        private void FixedUpdate()
        {
            if (_ghostFrames == null || _index >= _ghostFrames.Count)
                return;
            
            GhostFrame frame = _ghostFrames[_index];
            transform.SetPositionAndRotation(frame.Position, frame.Rotation);
            _index++;
        }
    }
}