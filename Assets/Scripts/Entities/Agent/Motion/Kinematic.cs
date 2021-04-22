using System;
using UnityEngine;
using Math = Utility.Math.Math;

namespace Entities.Steering {
    public class Kinematic {
        public const float DEFAULT_MAXIMUM_SPEED = 5;
        public const float DEFAULT_MAXIMUM_ANGULAR_SPEED = 360;
        public const float DEFAULT_MAXIMUM_ACCELERATION = 0.5f;
        public const float DEFAULT_MAXIMUM_ANGULAR_ACCELERATION = 180;
        private Vector3 acceleration;
        private Vector3 accumulatedAcceleration;
        private Vector3 accumulatedAngularAcceleration;
        private Vector3 accumulatedAngularVelocity;

        public Vector3 accumulatedVelocity;
        private Vector3 angularAcceleration;
        private Vector3 angularVelocity;
        private bool applyAccumulatedVelocities;

        private Vector3 rotation;
        private Vector3 velocity;

        public Kinematic(){
            MaximumSpeed = DEFAULT_MAXIMUM_SPEED;
            MaximumAngularSpeed = DEFAULT_MAXIMUM_ANGULAR_SPEED;
            MaximumAcceleration = DEFAULT_MAXIMUM_ACCELERATION;
            MaximumAngularAcceleration = DEFAULT_MAXIMUM_ANGULAR_ACCELERATION;

            Position = Vector3.zero;
            rotation = Vector3.zero;
            velocity = Vector3.zero;
            angularVelocity = Vector3.zero;
            acceleration = Vector3.zero;
            angularAcceleration = Vector3.zero;
        }

        public Kinematic(Kinematic kinematicSource){
            MaximumSpeed = kinematicSource.MaximumSpeed;
            MaximumAngularSpeed = kinematicSource.MaximumAngularSpeed;
            MaximumAcceleration = kinematicSource.MaximumAcceleration;
            MaximumAngularAcceleration = kinematicSource.MaximumAngularAcceleration;

            Position = kinematicSource.Position;
            rotation = kinematicSource.rotation;

            velocity = kinematicSource.velocity;
            angularVelocity = kinematicSource.angularVelocity;

            acceleration = kinematicSource.acceleration;
            angularAcceleration = kinematicSource.angularAcceleration;

            accumulatedVelocity = kinematicSource.accumulatedVelocity;
            accumulatedAngularVelocity = kinematicSource.accumulatedAngularVelocity;
            accumulatedAcceleration = kinematicSource.accumulatedAcceleration;
            accumulatedAngularAcceleration = kinematicSource.accumulatedAngularAcceleration;
        }

        public Vector3 Position { get; set; }

        public Vector3 Rotation {
            get => rotation;

            set => rotation = Math.WrapAngles(value);
        }

        public Vector3 HeadingVector => Quaternion.Euler(Rotation) * Vector3.forward;

        public Vector3 Velocity {
            get => velocity;

            set {
                velocity = value;

                if (MaximumMagnitudeIncludesVertical)
                    velocity = Math.LimitMagnitude(velocity, MaximumSpeed);
                else{
                    velocity.y = 0;
                    velocity = Math.LimitMagnitude(velocity, MaximumSpeed);
                    velocity.y = value.y;
                }
            }
        }

        public Vector3 AngularVelocity {
            get => angularVelocity;

            set => angularVelocity = Math.LimitMagnitude(value, MaximumAngularSpeed);
        }

        public Vector3 Acceleration {
            get => acceleration;

            set {
                acceleration = value;

                if (MaximumMagnitudeIncludesVertical)
                    acceleration = Math.LimitMagnitude(acceleration, MaximumAcceleration);
                else{
                    acceleration.y = 0;
                    acceleration = Math.LimitMagnitude(acceleration, MaximumAcceleration);
                    acceleration.y = value.y;
                }
            }
        }

        public Vector3 AngularAcceleration {
            get => angularAcceleration;

            set => angularAcceleration = Math.LimitMagnitude(value, MaximumAngularAcceleration);
        }

        public float Speed => velocity.magnitude;

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
            if (steering.Type == Steering.Types.Velocities)
                SetVelocities(steering.Linear, steering.Angular);
            else if (steering.Type == Steering.Types.Accelerations)
                SetAccelerations(steering.Linear, steering.Angular);
            else{
                throw new NotImplementedException(
                    string.Format("Steering type {0} is not implemented", steering.Type));
            }
        }

        public void AccumulateSteering(Steering steering){
            if (steering.Type == Steering.Types.Velocities)
                AccumulateVelocities(steering.Linear, steering.Angular);
            else if (steering.Type == Steering.Types.Accelerations)
                AccumulateAccelerations(steering.Linear, steering.Angular);
            else{
                throw new NotImplementedException(
                    string.Format("Steering type {0} is not implemented", steering.Type));
            }
        }

        public void SetVelocity(Vector3 velocity){
            applyAccumulatedVelocities = true;
            accumulatedVelocity = velocity;
        }

        public void SetAngularVelocity(Vector3 angularVelocity){
            applyAccumulatedVelocities = true;
            accumulatedAngularVelocity = angularVelocity;
        }

        public void SetAccelerations(Vector3 acceleration, Vector3 angularAcceleration){
            accumulatedAcceleration = acceleration;
            accumulatedAngularAcceleration = angularAcceleration;
        }

        public void SetVelocities(Vector3 velocity, Vector3 angularVelocity){
            applyAccumulatedVelocities = true;
            accumulatedVelocity = velocity;
            accumulatedAngularVelocity = angularVelocity;
        }

        public void AccumulateAccelerations(Vector3 acceleration, Vector3 angularAcceleration){
            accumulatedAcceleration += acceleration;
            accumulatedAngularAcceleration += angularAcceleration;
        }

        public void AccumulateVelocities(Vector3 velocity, Vector3 angularVelocity){
            applyAccumulatedVelocities = true;

            accumulatedVelocity.y = 0;

            accumulatedVelocity =
                Math.LimitMagnitude(accumulatedVelocity + velocity, MaximumSpeed);
            accumulatedAngularVelocity =
                Math.LimitMagnitude(accumulatedAngularVelocity + angularVelocity, MaximumAngularSpeed);
        }

        public void Update(float deltaTime){
            if (applyAccumulatedVelocities){
                Velocity = accumulatedVelocity;
                AngularVelocity = accumulatedAngularVelocity;
                applyAccumulatedVelocities = false;
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
                Math.LimitMagnitude(accumulatedAcceleration, MaximumAcceleration);
            AngularAcceleration =
                Math.LimitMagnitude(accumulatedAngularAcceleration, MaximumAngularAcceleration);

            accumulatedVelocity = Vector3.zero;
            accumulatedAngularVelocity = Vector3.zero;
            accumulatedAcceleration = Vector3.zero;
            accumulatedAngularAcceleration = Vector3.zero;
        }
    }
}