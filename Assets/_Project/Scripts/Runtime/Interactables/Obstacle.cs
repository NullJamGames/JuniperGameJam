using System;
using NJG.Runtime.Entity;
using NJG.Runtime.Managers;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Obstacle : MonoBehaviour, IBreakable
    {
        [SerializeField]
        private int _breakScore = 100;
        [SerializeField]
        private AudioClip[] _breakSound;
        [SerializeField]
        private GameObject _breakVFXPrefab;
        [SerializeField]
        private Vector3 _breakVFXOffset;
        
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Break(IEntity entity)
        {
            // TODO: Lets properly handle this..
            //_audioSource.PlayOneShot(_breakSound[UnityEngine.Random.Range(0, _breakSound.Length)]);
            entity.PlaySound(_breakSound[UnityEngine.Random.Range(0, _breakSound.Length)]);
            Instantiate(_breakVFXPrefab, transform.position + _breakVFXOffset, Quaternion.identity);
            
            gameObject.SetActive(false);
            GameManager.Instance.AddScore(_breakScore);
            Destroy(gameObject);
        }
    }
}