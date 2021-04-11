using UnityEngine;

public class SpawnPoint : Entity
{
    public Color color;
    public float radius = 1;
    
    public bool IsAvailable
    {
        get
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            
            foreach (Collider collider in colliders)
            {
                if (collider.GetComponent<Agent>() != null)
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}