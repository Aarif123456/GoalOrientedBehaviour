namespace GameBrains.AI
{
    using UnityEngine;
    
    public class Kinematic
    {
        public const float DEFAULT_MAXIMUM_SPEED = 5;
        public const float DEFAULT_MAXIMUM_ANGULAR_SPEED = 360;
        public const float DEFAULT_MAXIMUM_ACCELERATION = 0.5f;
        public const float DEFAULT_MAXIMUM_ANGULAR_ACCELERATION = 180;
        
        private Vector3 position;
        private Vector3 rotation;
        private Vector3 velocity;
        private Vector3 angularVelocity;
        private Vector3 acceleration;
        private Vector3 angularAcceleration;
        
        public Vector3 accumulatedVelocity;
        private Vector3 accumulatedAngularVelocity;
        private Vector3 accumulatedAcceleration;
        private Vector3 accumulatedAngularAcceleration;
        private bool applyAccumulatedVelocities;
        
        public Kinematic()
        {
            MaximumSpeed = DEFAULT_MAXIMUM_SPEED;
            MaximumAngularSpeed = DEFAULT_MAXIMUM_ANGULAR_SPEED;
            MaximumAcceleration = DEFAULT_MAXIMUM_ACCELERATION;
            MaximumAngularAcceleration = DEFAULT_MAXIMUM_ANGULAR_ACCELERATION;
            
            position = Vector3.zero;
            rotation = Vector3.zero;
            velocity = Vector3.zero;
            angularVelocity = Vector3.zero;
            acceleration = Vector3.zero;
            angularAcceleration = Vector3.zero;
        }
        
        public Kinematic(Kinematic kinematicSource)
        {
            MaximumSpeed = kinematicSource.MaximumSpeed;
            MaximumAngularSpeed = kinematicSource.MaximumAngularSpeed;
            MaximumAcceleration = kinematicSource.MaximumAcceleration;
            MaximumAngularAcceleration = kinematicSource.MaximumAngularAcceleration;
            
            position = kinematicSource.position;
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
        
        public Vector3 Position
        {
            get
            {
                return position;
            }
            
            set
            {
                position = value;
            }
        }
        
        public Vector3 Rotation
        {
            get
            {
                return rotation;
            }
            
            set
            {
                rotation = Math.WrapAngles(value);
            }
        }
        
        public Vector3 HeadingVector
        {
            get
            {
                return Quaternion.Euler(Rotation) * Vector3.forward;
            }
        }
        
        public Vector3 Velocity
        {
            get
            {
                return velocity;
            }
            
            set
            {
                velocity = value;
                
                if (MaximumMagnitudeIncludesVertical)
                {
                    velocity = Math.LimitMagnitude(velocity, MaximumSpeed);
                }
                else
                {
                    velocity.y = 0; 
                    velocity = Math.LimitMagnitude(velocity, MaximumSpeed);
                    velocity.y = value.y;    
                }
            }
        }
        
        public Vector3 AngularVelocity
        {
            get
            {
                return angularVelocity;
            }
            
            set
            {
                angularVelocity = Math.LimitMagnitude(value, MaximumAngularSpeed);
            }
        }
        
        public Vector3 Acceleration
        {
            get
            {
                return acceleration;
            }
            
            set
            {
                acceleration = value;
                
                if (MaximumMagnitudeIncludesVertical)
                {
                    acceleration = Math.LimitMagnitude(acceleration, MaximumAcceleration);
                }
                else
                {
                    acceleration.y = 0; 
                    acceleration = Math.LimitMagnitude(acceleration, MaximumAcceleration);
                    acceleration.y = value.y;    
                }
            }
        }
        
        public Vector3 AngularAcceleration
        {
            get
            {
                return angularAcceleration;
            }
            
            set
            {
                angularAcceleration = Math.LimitMagnitude(value, MaximumAngularAcceleration);
            }
        }
        
        public float Speed
        {
            get
            {
                return velocity.magnitude;
            }
        }
        
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
        
        public Vector3 Top
        {
            get
            {
                return Center + Vector3.up * Height * 0.5f;
            }
        }
        
        public Vector3 Bottom
        {
            get
            {
                return Center - Vector3.up * Height * 0.5f;
            }
        }
        
        public Vector3 Center
        {
            get
            {
                return Position + CenterOffset;
            }
        }
        
        public void SetSteering(Steering steering)
        {
            if (steering.Type == Steering.Types.Velocities)
            {
                SetVelocities(steering.Linear, steering.Angular);
            }
            else if (steering.Type == Steering.Types.Accelerations)
            {
                SetAccelerations(steering.Linear, steering.Angular);
            }
            else
            {
                throw new System.NotImplementedException(
                    string.Format("Steering type {0} is not implemented", steering.Type));
            }
        }
        
        public void AccumulateSteering(Steering steering)
        {
            if (steering.Type == Steering.Types.Velocities)
            {
                AccumulateVelocities(steering.Linear, steering.Angular);
            }
            else if (steering.Type == Steering.Types.Accelerations)
            {
                AccumulateAccelerations(steering.Linear, steering.Angular);
            }
            else
            {
                throw new System.NotImplementedException(
                    string.Format("Steering type {0} is not implemented", steering.Type));
            }
        }
            
        public void SetVelocity(Vector3 velocity)
        {
            applyAccumulatedVelocities = true;
            accumulatedVelocity = velocity;
        }
        
        public void SetAngularVelocity(Vector3 angularVelocity)
        {
            applyAccumulatedVelocities = true;
            accumulatedAngularVelocity = angularVelocity;
        }

        public void SetAccelerations(Vector3 acceleration, Vector3 angularAcceleration)
        {
            accumulatedAcceleration = acceleration;
            accumulatedAngularAcceleration = angularAcceleration;
        }

        public void SetVelocities(Vector3 velocity, Vector3 angularVelocity)
        {
            applyAccumulatedVelocities = true;
            accumulatedVelocity = velocity;
            accumulatedAngularVelocity = angularVelocity;
        }
        
        public void AccumulateAccelerations(Vector3 acceleration, Vector3 angularAcceleration)
        {
            accumulatedAcceleration += acceleration;
            accumulatedAngularAcceleration += angularAcceleration;
        }
        
        public void AccumulateVelocities(Vector3 velocity, Vector3 angularVelocity)
        {
            applyAccumulatedVelocities = true;
            
            accumulatedVelocity.y = 0;
            
            accumulatedVelocity = 
                Math.LimitMagnitude(accumulatedVelocity + velocity, MaximumSpeed);
            accumulatedAngularVelocity = 
                Math.LimitMagnitude(accumulatedAngularVelocity + angularVelocity, MaximumAngularSpeed);
            
        }
        
        public void Update(float deltaTime)
        {
            if (applyAccumulatedVelocities)
            {
                Velocity = accumulatedVelocity;
                AngularVelocity = accumulatedAngularVelocity;
                applyAccumulatedVelocities = false;
            }
            
            float halfDeltaTimeSquared = 0.5f * deltaTime * deltaTime;
            Vector3 positionOffset = (Velocity * deltaTime) + (Acceleration * halfDeltaTimeSquared);
            
            if (IsLockedPositionX)
            {
                positionOffset.x = 0;
            }
            
            if (IsLockedPositionY)
            {
                positionOffset.y = 0;
            }
            
            if (IsLockedPositionZ)
            {
                positionOffset.z = 0;
            }
            
            Position += positionOffset;
            
            Vector3 rotationOffset =
                (AngularVelocity * deltaTime) + (AngularAcceleration * halfDeltaTimeSquared);
            
            if (IsLockedRotationX)
            {
                rotationOffset.x = 0;
            }
            
            if (IsLockedRotationY)
            {
                rotationOffset.y = 0;
            }
            
            if (IsLockedRotationZ)
            {
                rotationOffset.z = 0;
            }
            
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