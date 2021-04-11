namespace GameBrains.AI
{
    using UnityEngine;
    
    /// <summary>
    /// Abstract base class for projectiles.
    /// </summary>
    /// <remarks>
    /// TODO: some code in the derived classes can be generalized and moved here.
    /// </remarks>
    public abstract class Projectile : MovingEntity
    {
        /// <summary>
        /// The position where this projectile impacts an object.
        /// </summary>
        /// <remarks>
        /// Since we cannot pass the <see cref="ImpactPoint"/> property as a ref parameter in some
        /// intersection tests, we make this field protected. This breaks encapsulation. 
        /// TODO: Might be useful to rethink the design of the intersection tests.
        /// </remarks>
        protected Vector3 impactPoint;
        
        /// <summary>
        /// Gets or sets a value indicating whether the projectile has impacted but is not yet dead
        /// (because it may be exploding outwards from the point of impact for example).
        /// </summary>
        public bool HasImpacted { get; protected set; }
        
        /// <summary>
        /// Gets or sets the agent that owns this projectile.
        /// </summary>
        public Agent Shooter { get; protected set; }
        
        public Weapon Weapon { get; protected set; }

        /// <summary>
        /// Gets or sets the target position this projectile is heading for.
        /// </summary>
        public Vector3 TargetPosition { get; protected set; }

        /// <summary>
        /// Gets or sets where the shot was fired from.
        /// </summary>
        public Vector3 OriginPosition { get; protected set; }

        /// <summary>
        /// Gets or sets the amount of damage a projectile of this type does.
        /// </summary>
        public float DamageInflicted { get; protected set; }

        /// <summary>
        /// Gets or sets the time when projectile was fired. This enables the shot to be rendered
        /// for a specific length of time.
        /// </summary>
        public float TimeOfCreation { get; set; }

        /// <summary>
        /// Gets or sets the position where this projectile impacts an object.
        /// </summary>
        public Vector3 ImpactPoint
        {
            get { return impactPoint; }
            set { impactPoint = value; }
        }
        
        public virtual void Spawn(Weapon weapon, Agent shooter, Vector3 originPosition, Vector3 targetPosition, float damageInflicted)
        {
            Weapon = weapon;
            Shooter = shooter;
            OriginPosition = originPosition;
            TargetPosition = targetPosition;
            DamageInflicted = damageInflicted;
            TimeOfCreation = Time.time;
            
            Spawn(OriginPosition);        
        }
    }
}