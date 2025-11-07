using UnityEngine;

public class EnemyDeathTracker : MonoBehaviour
{
    public EnemySpawner spawner;

    public void Init(EnemySpawner s) => spawner = s;

    // Called when the enemy's GameObject is destroyed (i.e., when EnemyController.Die() runs)
    private void OnDestroy()
    {
        // Check if the spawner is still active before trying to call its method
        if (spawner != null)
        {
            spawner.OnEnemyDied(transform.position);
        }
    }
}