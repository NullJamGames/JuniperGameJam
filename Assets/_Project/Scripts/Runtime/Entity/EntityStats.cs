using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    [Serializable]
    public class EntityStats
    {
        public enum Operation
        {
            Multiply,
            Divide
        }
        
        [BoxGroup("Movement"), SerializeField]
        public float _acceleration = 10f;
        [BoxGroup("Movement"), SerializeField]
        public float _maxSpeed = 30f;

        [BoxGroup("Spinning"), SerializeField]
        [Tooltip("Torque applied to the chair per unit of X input")]
        public float _spinTorque = 45f;
        [BoxGroup("Spinning"), SerializeField]
        [Tooltip("Maximum spin speed in radians per second")]
        public float _maxAngularSpeed = 10f;
        [BoxGroup("Spinning"), SerializeField]
        [Tooltip("How strongly the current spin speed pushes the player left/right")]
        public float _lateralInfluence = 2f;
        
        [FoldoutGroup("Physics"), SerializeField]
        [Tooltip("Fraction of spin speed kept after reversing on a wall hit (0 = all spin lost, 1 = full reversal)")]
        private float _wallBounceSpinRetention = 0.8f;
        
        [FoldoutGroup("Debug"), SerializeField, ReadOnly]
        private int _extraLives = 0;
        [FoldoutGroup("Debug"), SerializeField, ReadOnly]
        private float _accelerationMultiplier = 1f;
        [FoldoutGroup("Debug"), SerializeField, ReadOnly]
        private float _maxSpeedMultiplier = 1f;
        [FoldoutGroup("Debug"), SerializeField, ReadOnly]
        private float _spinTorqueMultiplier = 1f;
        [FoldoutGroup("Debug"), SerializeField, ReadOnly]
        private float _maxAngularSpeedMultiplier = 1f;
        [FoldoutGroup("Debug"), SerializeField, ReadOnly]
        private float _lateralInfluenceMultiplier = 1f;
        [FoldoutGroup("Debug"), SerializeField, ReadOnly]
        private float _gravityMultiplier = 2f;

        private Rigidbody _rigidbody;
        
        public void Initialize(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
            _rigidbody.maxAngularVelocity = GetMaxAngularSpeed();
        }
        
        public int AddExtraLives(int amount) => _extraLives += amount;
        public bool TryRemoveExtraLives(int amount)
        {
            if (_extraLives >= amount)
            {
                _extraLives -= amount;
                return true;
            }
            return false;
        }
        public void ResetExtraLives() => _extraLives = 0;
        public int GetExtraLives() => _extraLives;
        
        public void ApplyAccelerationMultiplier(Operation op, float multiplier)
        {
            _accelerationMultiplier = ProcessMultiplier(op, _accelerationMultiplier, multiplier);
        }
        
        public void ApplyMaxSpeedMultiplier(Operation op, float multiplier)
        {
            _maxSpeedMultiplier = ProcessMultiplier(op, _maxSpeedMultiplier, multiplier);
        }
        
        public void ApplySpinTorqueMultiplier(Operation op, float multiplier)
        {
            _spinTorqueMultiplier = ProcessMultiplier(op, _spinTorqueMultiplier, multiplier);
        }
        
        public void ApplyMaxAngularSpeedMultiplier(Operation op, float multiplier)
        {
            _maxAngularSpeedMultiplier = ProcessMultiplier(op, _maxAngularSpeedMultiplier, multiplier);
            _rigidbody.maxAngularVelocity = GetMaxAngularSpeed();
        }
        
        public void ApplyLateralInfluenceMultiplier(Operation op, float multiplier)
        {
            _lateralInfluenceMultiplier = ProcessMultiplier(op, _lateralInfluenceMultiplier, multiplier);
        }
        
        public void ApplyGravityMultiplier(Operation op, float multiplier)
        {
            _gravityMultiplier = ProcessMultiplier(op, _gravityMultiplier, multiplier);
        }
        
        public float GetAcceleration() => _acceleration * _accelerationMultiplier;
        public float GetMaxSpeed() => _maxSpeed * _maxSpeedMultiplier;
        public float GetSpinTorque() => _spinTorque * _spinTorqueMultiplier;
        public float GetMaxAngularSpeed() => _maxAngularSpeed * _maxAngularSpeedMultiplier;
        public float GetLateralInfluence() => _lateralInfluence * _lateralInfluenceMultiplier;
        public float GetWallBounceSpinRetention() => _wallBounceSpinRetention;
        public float GetGravityMultiplier() => _gravityMultiplier;

        private float ProcessMultiplier(Operation op, float current, float incoming)
        {
            return op switch
            {
                Operation.Multiply => current * incoming,
                Operation.Divide => current / incoming,
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }
    }
}