using UnityEngine;

public abstract class TriggerTimeLimited : Trigger
{
    /// <summary>
    /// Gets or sets the lifetime of the trigger.
    /// </summary>
    public float Lifetime { get; protected set; }

    public override void Update()
    {
        base.Update();

        Lifetime -= Time.deltaTime;
        
        if (Lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
