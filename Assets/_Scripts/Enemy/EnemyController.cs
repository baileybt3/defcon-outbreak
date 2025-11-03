using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private float radiusOfSatisfaction = 1f; 

    [Header("Attack")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRate = 1.5f; 
    private float lastAttackTime;

    private Animator animator;
    private bool isAttacking = false;
    private bool isDead = false;

    
    // --- Damage Feedback (for the optional damage popup feature) ---
    [Header("Damage Feedback")]
    public GameObject damageTextPrefab; 
    
    // --- Unity Lifecycle Methods ---

    private void Start() // MODIFIED: Simplified
    {
        currentHealth = maxHealth;
        lastAttackTime = Time.time;
        animator = GetComponent<Animator>();
        animator.SetInteger("State", 0);
    }

    void Update()
    {
        if (isDead) return;

        // Check if the player exists using the static Instance
        if (PlayerController.Instance != null && currentHealth > 0)
        {
            MoveOrAttack();
        }

        // Animations
        if (currentHealth <= 0)
        {
            animator.SetInteger("State", 2); //Death
        }
        else if (isAttacking)
        {
            animator.SetInteger("State", 1); //Attack
        }
        else
        {
            animator.SetInteger("State", 0); //Walk
        }
    }
    
    // --- Combat and Movement ---

    void MoveOrAttack()
    {
        // Use PlayerController.Instance.transform for player position
        Vector3 playerPos = PlayerController.Instance.transform.position;
        
        Vector3 towardsTarget = playerPos - transform.position; // Use local transform
        towardsTarget.y = 0f;

        float distance = towardsTarget.magnitude;

        if (distance <= radiusOfSatisfaction)
        {
            AttackPlayer();
            isAttacking = true;
            return;
        }
        else
        {
            isAttacking = false;
        }

            // --- Movement Logic (Updated to use transform) ---
            towardsTarget = towardsTarget.normalized;

        Quaternion targetRotation = Quaternion.LookRotation(towardsTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.1f);

        float targetSpeed = moveSpeed;

        GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * targetSpeed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        if (Time.time > lastAttackTime + attackRate)
        {
            // Call TakeDamage on the player via the static Instance
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.TakeDamage(attackDamage);
            }
            lastAttackTime = Time.time;
        }
    }

    // --- Public Methods (For Bullets and Damage Popups) ---

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0 || isDead) return;
        
        // --- Damage Popup Logic (from the previous step) ---
        if (damageTextPrefab != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.up * 1f; 
            GameObject damageTextObject = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);
            
            DamageText damageText = damageTextObject.GetComponent<DamageText>();
            if (damageText != null)
            {
                damageText.SetDamageValue(damageAmount);
            }
        }
        
        currentHealth -= damageAmount;
        Debug.Log("Enemy took " + damageAmount + " damage. Remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " has died!");
        animator.SetInteger("State", 2);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;

        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}