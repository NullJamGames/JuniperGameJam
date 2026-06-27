using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class EntityVisual : MonoBehaviour
    {
        [SerializeField]
        private Renderer[] _renderers;
        [SerializeField]
        private Renderer _playerBodyVisual;
        [SerializeField]
        private GameObject _ragdollPrefab;
        [SerializeField]
        private ParticleSystem _lostHeartVFX;
        [SerializeField]
        private ParticleSystem _sparkVFX;
        
        private GameObject _ragdollInstance;
        
        private static readonly int _isInvincibleId = Shader.PropertyToID("_isInvincible");

        public void SetInvincibleVisual(bool isInvincible)
        {
            foreach (Renderer renderer in _renderers)
            {
                renderer.material.SetFloat(_isInvincibleId, isInvincible ? 1f : 0f);
            }
        }

        public void Ragdoll()
        {
            _playerBodyVisual.enabled = false;
            _ragdollInstance = Instantiate(_ragdollPrefab, transform.position, transform.rotation);
        }

        public void LostHeart()
        {
            _lostHeartVFX.gameObject.SetActive(true);
            _lostHeartVFX.Stop();
            _lostHeartVFX.Play();
        }
        
        public void Spark(Vector3 position)
        {
            _sparkVFX.gameObject.SetActive(true);
            _sparkVFX.transform.position = position;
            _sparkVFX.Stop();
            _sparkVFX.Play();
        }

        public void ResetVisual()
        {
            if (_ragdollInstance != null)
            {
                Destroy(_ragdollInstance);
                _ragdollInstance = null;
            }
            _playerBodyVisual.enabled = true;
        }
    }
}