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
        [Header("Movement")]
        [SerializeField]
        private float _acceleration = 10f;
        [SerializeField]
        private float _maxSpeed = 30f;

        [Header("Spinning")]
        [Tooltip("Torque applied to the chair per unit of X input")]
        [SerializeField]
        private float _spinTorque = 45f;
        [Tooltip("Maximum spin speed in radians per second")]
        [SerializeField]
        private float _maxAngularSpeed = 10f;
        [Tooltip("How strongly the current spin speed pushes the player left/right")]
        [SerializeField]
        private float _lateralInfluence = 5f;

        [Header("Wall Bounce")]
        [Tooltip("Fraction of spin speed kept after reversing on a wall hit (0 = all spin lost, 1 = full reversal)")]
        [SerializeField]
        private float _wallBounceSpinRetention = 0.8f;

        [Header("Other")]
        [SerializeField, ValueDropdown(nameof(GetLayerDropdownItems))]
        private int _defaultLayer;
        [SerializeField, ValueDropdown(nameof(GetLayerDropdownItems))]
        private int _obstacleLayer;
        [SerializeField, ValueDropdown(nameof(GetLayerDropdownItems))]
        private int _breakableObstacleLayer;
        [SerializeField, ValueDropdown(nameof(GetLayerDropdownItems))]
        private int _pickupableLayer;
        [SerializeField, ValueDropdown(nameof(GetLayerDropdownItems))]
        private int _invincibleLayer;
        [field: SerializeField]
        public bool IsInvincible { get; private set; }
        
        private CapsuleCollider _collider;
        private Rigidbody _rigidbody;
        private IInputProvider _input;
        private EntityModifiers _modifiers;

        private bool _isMoving = true;

        private float _accelerationMultiplier = 1f;
        private float _maxSpeedMultiplier = 1f;
        
        public Vector3 Position => transform.position;

        public event Action OnStoppedMoving;
        
        private void Awake()
        {
            _collider = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();
            _modifiers = new EntityModifiers(this);
            
            _rigidbody.maxAngularVelocity = _maxAngularSpeed;
        }

        private void OnEnable()
        {
            EventBus.StartListening<ChunkZPositionResetEvent>(OnChunkZPositionReset);
        }

        private void Update()
        {
            if (transform.position.z >= GameManager.Instance.EnvironmentChunkTriggerZ)
            {
                EventBus.TriggerEvent(new RequestNextChunkEvent());
            }
            
            _modifiers.ProcessModifiers();
        }

        private void FixedUpdate()
        {
            if (!_isMoving || _input == null)
                return;
            
            Vector2 moveInput = _input.GetMovement();
            float xMovement = moveInput.x;

            // Spin the chair around Y based on input — accumulates so full rotations are possible
            _rigidbody.AddTorque(Vector3.up * (xMovement * _spinTorque), ForceMode.Acceleration);

            // Always propel forward down the hallway (world space)
            _rigidbody.AddForce(Vector3.forward * (_acceleration * _accelerationMultiplier), ForceMode.Acceleration);

            // Current spin speed drives lateral drift — faster spin = more left/right push
            float spinY = _rigidbody.angularVelocity.y;
            _rigidbody.AddForce(Vector3.right * (spinY * _lateralInfluence), ForceMode.Acceleration);

            if (_rigidbody.linearVelocity.magnitude > (_maxSpeed * _maxSpeedMultiplier))
            {
                _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * (_maxSpeed * _maxSpeedMultiplier);
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            // Look for a contact whose normal is mostly along the X axis (a side wall)
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Mathf.Abs(contact.normal.x) > 0.5f)
                {
                    // Reverse Y spin to simulate the chair bouncing off the wall
                    Vector3 av = _rigidbody.angularVelocity;
                    av.y = -av.y * _wallBounceSpinRetention;
                    _rigidbody.angularVelocity = av;
                    break;
                }

                // Hit head on object
                if (contact.normal.z < -0.5f && !IsInvincible && 
                    (collision.gameObject.layer == _obstacleLayer || collision.gameObject.layer == _breakableObstacleLayer))
                {
                    // front collision
                    _isMoving = false;
                    _collider.material = null;
                    OnStoppedMoving?.Invoke();
                    break;
                }
            }
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
        }

        private void OnChunkZPositionReset(ChunkZPositionResetEvent e)
        {
            Vector3 playerPosition = transform.position;
            SetPosition(playerPosition.WithZ(playerPosition.z - e.ZPositionResetAmount));
        }

        public void Init(IInputProvider inputProvider)
        {
            _input = inputProvider;
        }

        public void SetPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }

        public void ApplyModifier(BaseModifierSO modifierData)
        {
            _modifiers.AddModifier(modifierData);
        }

        public void SetInvincible(bool isInvincible)
        {
            IsInvincible = isInvincible;
            gameObject.layer = isInvincible ? _invincibleLayer : _defaultLayer;
        }
        
        public void SetAccelerationMultiplier(float accelerationMultiplier)
        {
            _accelerationMultiplier = accelerationMultiplier;
        }
        
        public void SetMaxSpeedMultiplier(float maxSpeedMultiplier)
        {
            _maxSpeedMultiplier = maxSpeedMultiplier;
        }

        private IEnumerable<ValueDropdownItem<int>> GetLayerDropdownItems()
        {
            return Tools.GetLayerDropdownItems();
        }
    }
}
