using System;
using System.Collections.Generic;
using NJG.Runtime.Events;
using NJG.Runtime.Input;
using NJG.Runtime.Interactables;
using NJG.Runtime.Managers;
using NJG.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class PlayerEntity : MonoBehaviour, IEntity
    {
        [FoldoutGroup("Stats"), SerializeField, HideLabel]
        private EntityStats _stats;

        [FoldoutGroup("Layers"), SerializeField, ValueDropdown(nameof(GetLayerDropdownItems))]
        private int _defaultLayer;
        [FoldoutGroup("Layers"), SerializeField, ValueDropdown(nameof(GetLayerDropdownItems))]
        private int _groundLayer;
        [FoldoutGroup("Layers"), SerializeField, ValueDropdown(nameof(GetLayerDropdownItems))]
        private int _obstacleLayer;
        [FoldoutGroup("Layers"), SerializeField, ValueDropdown(nameof(GetLayerDropdownItems))]
        private int _breakableObstacleLayer;
        [FoldoutGroup("Layers"), SerializeField, ValueDropdown(nameof(GetLayerDropdownItems))]
        private int _pickupableLayer;
        [FoldoutGroup("Layers"), SerializeField, ValueDropdown(nameof(GetLayerDropdownItems))]
        private int _invincibleLayer;
        
        [field: FoldoutGroup("Other"), SerializeField]
        public bool IsInvincible { get; private set; }
        [FoldoutGroup("Other"), SerializeField]
        private PhysicsMaterial _slipperyMat;
        
        [FoldoutGroup("Debug"), SerializeField, ReadOnly]
        private bool _isGrounded = true;
        
        private CapsuleCollider _collider;
        private Rigidbody _rigidbody;
        private IInputProvider _input;
        private EntityModifiers _modifiers;
        private EntityVisual _visual;
        private EntityAudio _audio;

        private bool _isMoving = true;

        private readonly List<GhostFrame> _ghostFrames = new();
        
        public Vector3 Position => transform.position;
        public EntityStats Stats => _stats;

        public event Action OnStoppedMoving;
        
        private void Awake()
        {
            _collider = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();
            _modifiers = new EntityModifiers(this, _stats);
            _visual = GetComponent<EntityVisual>();
            _audio = GetComponent<EntityAudio>();
            
            _stats.Initialize(_rigidbody);
        }

        private void OnEnable()
        {
            EventBus.StartListening<ChunkZPositionResetEvent>(OnChunkZPositionReset);
            EventBus.StartListening<NewRunEvent>(OnNewRun);
            EventBus.StartListening<EndRunRequestEvent>(OnEndRunRequested);
        }

        private void Update()
        {
            if (transform.position.z >= GameManager.Instance.EnvironmentChunkTriggerZ)
            {
                EventBus.TriggerEvent(new RequestNextChunkEvent());
                GameManager.Instance.AddScore();
            }
            
            _modifiers.ProcessModifiers();
        }

        private void FixedUpdate()
        {
            if (!_isMoving || _input == null)
                return;
            
            // Ghost recording
            _ghostFrames.Add(new GhostFrame(transform.position, transform.rotation));
            
            Vector2 moveInput = _input.GetMovement();
            float xMovement = moveInput.x;

            // Spin the chair around Y based on input — accumulates so full rotations are possible
            _rigidbody.AddTorque(Vector3.up * (xMovement * _stats.GetSpinTorque()), ForceMode.Acceleration);

            // Always propel forward down the hallway (world space)
            _rigidbody.AddForce(Vector3.forward * (_stats.GetAcceleration()), ForceMode.Acceleration);

            // Current spin speed drives lateral drift — faster spin = more left/right push
            float spinY = _rigidbody.angularVelocity.y;
            _rigidbody.AddForce(Vector3.right * (spinY * _stats.GetLateralInfluence()), ForceMode.Acceleration);
            
            // Gravity
            if (_rigidbody.useGravity)
            {
                Vector3 gravity = (Physics.gravity * _stats.GetGravityMultiplier()) - Physics.gravity;
                _rigidbody.AddForce(gravity, ForceMode.Acceleration);
            }

            if (_rigidbody.linearVelocity.magnitude > (_stats.GetMaxSpeed()))
            {
                _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * (_stats.GetMaxSpeed());
            }
            
            _audio.ToggleWheelSound(_isGrounded && _rigidbody.linearVelocity.magnitude > 0.1f);
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            // Look for a contact whose normal is mostly along the X axis (a side wall)
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Mathf.Abs(contact.normal.x) > 0.5f)
                {
                    _audio.PlayHitSFX();
                    _visual.Spark(contact.point);
                    // Reverse Y spin to simulate the chair bouncing off the wall
                    Vector3 av = _rigidbody.angularVelocity;
                    av.y = -av.y * _stats.GetWallBounceSpinRetention();
                    _rigidbody.angularVelocity = av;
                    break;
                }

                // Hit front collision
                if (contact.normal.z < -0.5f && !IsInvincible && 
                    _isMoving &&
                    (collision.gameObject.layer == _obstacleLayer || collision.gameObject.layer == _breakableObstacleLayer))
                {
                    if (_modifiers.TryRemoveModifierByType<LifeModifierSO>())
                    {
                        _audio.PlayHitSFX();
                        _visual.Spark(contact.point);
                        _visual.LostHeart();
                        break;
                    }
                    
                    _audio.PlayCrashSFX();
                    _visual.Ragdoll();
                    EndRun(false);
                    break;
                }
                else
                {
                    _audio.PlayHitSFX();
                    _visual.Spark(contact.point);
                }
            }
        }

        private void OnCollisionStay(Collision other)
        {
            _isGrounded = other.gameObject.layer == _groundLayer;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == _pickupableLayer && other.TryGetComponent(out IPickupable pickupable))
            {
                pickupable.Pickup(this);
            }
        }

        private void OnDisable()
        {
            EventBus.StopListening<ChunkZPositionResetEvent>(OnChunkZPositionReset);
            EventBus.StopListening<NewRunEvent>(OnNewRun);
            EventBus.StopListening<EndRunRequestEvent>(OnEndRunRequested);
        }

        private void EndRun(bool instantStop)
        {
            _audio.ToggleWheelSound(false);
            _isMoving = false;
            if (instantStop) _rigidbody.linearVelocity = Vector3.zero;
            _collider.material = null;
            GameManager.Instance.NewGhostFrames(_ghostFrames);
            OnStoppedMoving?.Invoke();
        }

        private void OnChunkZPositionReset(ChunkZPositionResetEvent e)
        {
            Vector3 playerPosition = transform.position;
            SetPosition(playerPosition.WithZ(playerPosition.z - e.ZPositionResetAmount));
        }

        private void OnEndRunRequested(EndRunRequestEvent e)
        {
            EndRun(true);
        }

        public void Init(IInputProvider inputProvider)
        {
            _input = inputProvider;
        }

        public void SetPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }
        
        public void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        public void ApplyModifier(Modifier modifier)
        {
            _modifiers.AddModifier(modifier);
        }

        public void PlaySound(AudioClip clip)
        {
            _audio.PlaySFX(clip);
        }

        public void SetInvincible(bool isInvincible)
        {
            IsInvincible = isInvincible;
            gameObject.layer = isInvincible ? _invincibleLayer : _defaultLayer;
            _visual.SetInvincibleVisual(isInvincible);
        }

        private void OnNewRun(NewRunEvent e)
        {
            _visual.ResetVisual();
            SetPosition(Vector3.zero);
            SetRotation(Quaternion.identity);
            _modifiers.RemoveAllModifiers();
            _stats.ResetExtraLives();
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _isMoving = true;
            _collider.material = _slipperyMat;
            
            _ghostFrames.Clear();
        }

        private IEnumerable<ValueDropdownItem<int>> GetLayerDropdownItems()
        {
            return Tools.GetLayerDropdownItems();
        }
    }
}
