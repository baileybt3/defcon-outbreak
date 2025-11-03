using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float lifetime = 5f; // Destroy the bullet after 5 seconds if it misses

    private void Start()
    {
        // Ensure the bullet is destroyed eventually even if it misses everything
        Destroy(gameObject, lifetime);
    }

    // This function is called when the bullet hits something
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Try to get the EnemyController component from the object we hit
        EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();

        if (enemy != null)
        {
            // 2. If it has the component, call its TakeDamage method
            enemy.TakeDamage(damageAmount);
        }

        // 3. Always destroy the bullet once it hits anything (enemy, wall, ground)
        Destroy(gameObject);
    }
}