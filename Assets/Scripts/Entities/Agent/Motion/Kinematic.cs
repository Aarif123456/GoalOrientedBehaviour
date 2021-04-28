using System;
using UnityEngine;
using Math = Utility.Math.Math;

namespace Entities.Steering {
    public class Kinematic {
        private const float _DEFAULT_MAXIMUM_SPEED = 5;
        private const float _DEFAULT_MAXIMUM_ANGULAR_SPEED = 360;
        private const float _DEFAULT_MAXIMUM_ACCELERATION = 0.5f;
        private const float _DEFAULT_MAXIMUM_ANGULAR_ACCELERATION = 180;
        private Vector3 _acceleration;
        private Vector3 _accumulatedAcceleration;
        private Vector3 _accumulatedAngularAcceleration;
        private Vector3 _accumulatedAngularVelocity;

        private Vector3 _accumulatedVelocity;
        private Vector3 _angularAcceleration;
        private Vector3 _angularVelocity;
        private bool _applyAccumulatedVelocities;

        private Vector3 _rotation;
        private Vector3 _velocity;

        public Kinematic(){
            MaximumSpeed = _DEFAULT_MAXIMUM_SPEED;
            MaximumAngularSpeed = _DEFAULT_MAXIMUM_ANGULAR_SPEED;
            MaximumAcceleration = _DEFAULT_MAXIMUM_ACCELERATION;
            MaximumAngularAcceleration = _DEFAULT_MAXIMUM_ANGULAR_ACCELERATION;

            Position = Vector3.zero;
            _rotation = Vector3.zero;
            _velocity = Vector3.zero;
            _angularVelocity = Vector3.zero;
            _acceleration = Vector3.zero;
            _angularAcceleration = Vector3.zero;
        }

        public Kinematic(Kinematic kinematicSource){
            MaximumSpeed = kinematicSource.MaximumSpeed;
            MaximumAngularSpeed = kinematicSource.MaximumAngularSpeed;
            MaximumAcceleration = kinematicSource.MaximumAcceleration;
            MaximumAngularAcceleration = kinematicSource.MaximumAngularAcceleration;

            Position = kinematicSource.Position;
            _rotation = kinematicSource._rotation;

            _velocity = kinematicSource._velocity;
            _angularVelocity = kinematicSource._angularVelocity;

            _acceleration = kinematicSource._acceleration;
            _angularAcceleration = kinematicSource._angularAcceleration;

            _accumulatedVelocity = kinematicSource._accumulatedVelocity;
            _accumulatedAngularVelocity = kinematicSource._accumulatedAngularVelocity;
            _accumulatedAcceleration = kinematicSource._accumulatedAcceleration;
            _accumulatedAngularAcceleration = kinematicSource._accumulatedAngularAcceleration;
        }

        public Vector3 Position { get; set; }

        public Vector3 Rotation {
            get => _rotation;

            set => _rotation = Math.WrapAngles(value);
        }

        public Vector3 HeadingVector => Quaternion.Euler(Rotation) * Vector3.forward;

        public Vector3 Velocity {
            get => _velocity;

            set {
                _velocity = value;

                if (MaximumMagnitudeIncludesVertical)
                    _velocity = Math.LimitMagnitude(_velocity, MaximumSpeed);
                else{
                    _velocity.y = 0;
                    _velocity = Math.LimitMagnitude(_velocity, MaximumSpeed);
                    _velocity.y = value.y;
                }
            }
        }

        public Vector3 AngularVelocity {
            get => _angularVelocity;

            set => _angularVelocity = Math.LimitMagnitude(value, MaximumAngularSpeed);
        }

        public Vector3 Acceleration {
            get => _acceleration;

            set {
                _acceleration = value;

                if (MaximumMagnitudeIncludesVertical)
                    _acceleration = Math.LimitMagnitude(_acceleration, MaximumAcceleration);
                else{
                    _acceleration.y = 0;
                    _acceleration = Math.LimitMagnitude(_acceleration, MaximumAcceleration);
                    _acceleration.y = value.y;
                }
            }
        }

        public Vector3 AngularAcceleration {
            get => _angularAcceleration;

            set => _angularAcceleration = Math.LimitMagnitude(value, MaximumAngularAcceleration);
        }

        public float Speed => _velocity.magnitude;

        public float MaximumSpeed { get; set; }

        public float MaximumAngularSpeed { get; set; }

        public float MaximumAcceleration { get; set; }

        public float MaximumAngularAcceleration { get; set; }

        public bool MaximumMagnitudeIncludesVertical { get; set; }

        public bool IsLockedPositionX { get; set; }

        public bool IsLockedPositionY { get; set; }

        public bool IsLockedPositionZ { get; set; }

        public bool IsLockedRotationX { get; set; }

        public bool IsLockedRotationY { get; set; }

        public bool IsLockedRotationZ { get; set; }

        public float Radius { get; set; }

        public float Height { get; set; }

        public Vector3 CenterOffset { get; set; }

        public Vector3 Top => Center + Vector3.up * Height * 0.5f;

        public Vector3 Bottom => Center - Vector3.up * Height * 0.5f;

        public Vector3 Center => Position + CenterOffset;

        public void SetSteering(Steering steering){
            switch (steering.Type){
                case Steering.Types.Velocities:
                    SetVelocities(steering.Linear, steering.Angular);
                    break;
                case Steering.Types.Accelerations:
                    SetAccelerations(steering.Linear, steering.Angular);
                    break;
                default:
                    throw new NotImplementedException(
                        $"Steering type {steering.Type} is not implemented");
            }
        }

        public void AccumulateSteering(Steering steering){
            switch (steering.Type){
                case Steering.Types.Velocities:
                    AccumulateVelocities(steering.Linear, steering.Angular);
                    break;
                case Steering.Types.Accelerations:
                    AccumulateAccelerations(steering.Linear, steering.Angular);
                    break;
                default:
                    throw new NotImplementedException(
                        $"Steering type {steering.Type} is not implemented");
            }
        }

        public void SetVelocity(Vector3 velocity){
            _applyAccumulatedVelocities = true;
            _accumulatedVelocity = velocity;
        }

        public void SetAngularVelocity(Vector3 angularVelocityAccumulated){
            _applyAccumulatedVelocities = true;
            _accumulatedAngularVelocity = angularVelocityAccumulated;
        }

        public void SetAccelerations(Vector3 accelerationAccumulated, Vector3 angularVelocityAccumulated){
            _accumulatedAcceleration = accelerationAccumulated;
            _accumulatedAngularAcceleration = angularVelocityAccumulated;
        }

        public void SetVelocities(Vector3 velocity, Vector3 angularVelocityAccumulated){
            _applyAccumulatedVelocities = true;
            _accumulatedVelocity = velocity;
            _accumulatedAngularVelocity = angularVelocityAccumulated;
        }

        public void AccumulateAccelerations(Vector3 acceleration, Vector3 angularAcceleration){
            _accumulatedAcceleration += acceleration;
            _accumulatedAngularAcceleration += angularAcceleration;
        }

        public void AccumulateVelocities(Vector3 velocity, Vector3 angularVelocity){
            _applyAccumulatedVelocities = true;

            _accumulatedVelocity.y = 0;

            _accumulatedVelocity =
                Math.LimitMagnitude(_accumulatedVelocity + velocity, MaximumSpeed);
            _accumulatedAngularVelocity =
                Math.LimitMagnitude(_accumulatedAngularVelocity + angularVelocity, MaximumAngularSpeed);
        }

        public void Update(float deltaTime){
            if (_applyAccumulatedVelocities){
                Velocity = _accumulatedVelocity;
                AngularVelocity = _accumulatedAngularVelocity;
                _applyAccumulatedVelocities = false;
            }

            var halfDeltaTimeSquared = 0.5f * deltaTime * deltaTime;
            var positionOffset = Velocity * deltaTime + Acceleration * halfDeltaTimeSquared;

            if (IsLockedPositionX) positionOffset.x = 0;

            if (IsLockedPositionY) positionOffset.y = 0;

            if (IsLockedPositionZ) positionOffset.z = 0;

            Position += positionOffset;

            var rotationOffset =
                AngularVelocity * deltaTime + AngularAcceleration * halfDeltaTimeSquared;

            if (IsLockedRotationX) rotationOffset.x = 0;

            if (IsLockedRotationY) rotationOffset.y = 0;

            if (IsLockedRotationZ) rotationOffset.z = 0;

            Rotation += rotationOffset;

            Velocity =
                Math.LimitMagnitude(Velocity + Acceleration * deltaTime, MaximumSpeed);
            AngularVelocity =
                Math.LimitMagnitude(AngularVelocity + AngularAcceleration * deltaTime, MaximumAngularSpeed);

            Acceleration =
                Math.LimitMagnitude(_accumulatedAcceleration, MaximumAcceleration);
            AngularAcceleration =
                Math.LimitMagnitude(_accumulatedAngularAcceleration, MaximumAngularAcceleration);

            _accumulatedVelocity = Vector3.zero;
            _accumulatedAngularVelocity = Vector3.zero;
            _accumulatedAcceleration = Vector3.zero;
            _accumulatedAngularAcceleration = Vector3.zero;
        }
    }
}