using UnityEngine;

// This script will be dynamically added to enemies when they are spawned.
public class EnemyDeathTracker : MonoBehaviour
{
    // The spawner that created this enemy
    public EnemySpawner spawner;

    // Called when the enemy's GameObject is destroyed (i.e., when EnemyController.Die() runs)
    private void OnDestroy()
    {
        // Check if the spawner is still active before trying to call its method
        if (spawner != null)
        {
            spawner.EnemyDied();
        }
    }
}