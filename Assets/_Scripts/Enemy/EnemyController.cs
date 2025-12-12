using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    // FSM
    private enum EnemyState
    {
        Idle, Alert, Chase, Attack, Dead
    }

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float radiusOfSatisfaction = 2.0f;
    private Rigidbody rb;
    private Vector3 pendingMove;

    [Header("Attack")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRate = 1.5f; 
    private float lastAttackTime;

    [Header("Pathfinding")]
    [SerializeField] private WorldDecomposer worldDecomposer;
    [SerializeField] private float repathInterval = 1.0f;
    [SerializeField] private float repathDistanceThreshold = 1.5f;
    [SerializeField] private float repathJitter = 0.3f;
    private float lastPathTime;
    private float nextRepathTime;
    private Vector3 lastPathTarget;
    private List<Vector3> pathPoints;
    private int currentPathIndex = 0;

    [Header("Zombie perception")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float loseInterestRadius = 25f;
    [SerializeField] private float alertDuration = 1.0f;
    private float alertEndTime;

    [Header("Crowd Behaviour")]
    [SerializeField] private float separationRadius = 1.2f;
    [SerializeField] private float separationStrength = 0.5f;

    [Header("Animations")]
    private Animator animator;
    private bool isAttacking = false;
    private bool isDead = false;

    [Header("Rewards")]
    [SerializeField] private int killReward = 25;

    [Header("FSM")]
    [SerializeField] private EnemyState startingState = EnemyState.Idle;
    private EnemyState currentState;

    // Horde
    private Vector3 formationOffset;
    private float speedMultiplier = 1.0f;

    //Enemy global list
    public static List<EnemyController> AllEnemies = new List<EnemyController>();

    private void OnEnable()
    {
        AllEnemies.Add(this);
    }

    private void OnDisable()
    {
        AllEnemies.Remove(this);
    }

    private void Start()
    {
        currentHealth = maxHealth;
        lastAttackTime = Time.time - attackRate;
        lastPathTime = -repathInterval;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if(worldDecomposer == null)
        {
            worldDecomposer = FindFirstObjectByType<WorldDecomposer>();
        }

        // Zombie differences
        speedMultiplier = Random.Range(0.9f, 1.1f);
        formationOffset = GetRandomFormationOffset();
        nextRepathTime = Time.time + Random.Range(0f, repathInterval);

        currentState = startingState;

    }
    private void FixedUpdate()
    {
        if (isDead) return;

        if(!isAttacking && pendingMove.sqrMagnitude > 0f)
        {
            rb.MovePosition(rb.position + pendingMove * Time.fixedDeltaTime);
        }
    }
    void Update()
    {
        if (isDead) return;

        if (PlayerController.Instance == null)
        {
            return;
        }

        UpdateStateMachine();
        UpdateAnimations();
    }

    // Finite State Machine
    private void UpdateStateMachine()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Alert:
                UpdateAlert();
                break;
            case EnemyState.Chase:
                UpdateChase();
                break;
            case EnemyState.Attack:
                UpdateAttack();
                break;
            case EnemyState.Dead:
                break;
        }
    }


    private void EnterState(EnemyState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        switch (newState)
        {
            case EnemyState.Idle:
                pendingMove = Vector3.zero;
                isAttacking = false;
                break;

            case EnemyState.Alert:
                pendingMove = Vector3.zero;
                isAttacking = false;
                alertEndTime = Time.time + alertDuration;
                break;

            case EnemyState.Chase:
                isAttacking = false;
                break;

            case EnemyState.Attack:
                pendingMove = Vector3.zero;
                isAttacking = true;
                break;

            case EnemyState.Dead:
                pendingMove = Vector3.zero;
                isAttacking = false;
                break;
        }
    }

    // FSM States
    private void UpdateIdle()
    {
        pendingMove = Vector3.zero;
        isAttacking = false;

        float distance = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
        if(distance <= detectionRadius)
        {
            EnterState(EnemyState.Alert);
        }
    }

    private void UpdateAlert()
    {
        pendingMove = Vector3.zero;
        isAttacking = false;

        Vector3 playerPos = PlayerController.Instance.transform.position;
        Vector3 toPlayer = playerPos - transform.position;
        toPlayer.y = 0f;

        // Turn to face player
        if(toPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toPlayer.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        float distance = toPlayer.magnitude;

        if(distance > loseInterestRadius)
        {
            EnterState(EnemyState.Idle);
            return;
        }

        if (Time.time >= alertEndTime)
        {
            EnterState(EnemyState.Chase);
        }
    }

    private void UpdateChase()
    {
        if(worldDecomposer == null)
        {
            pendingMove = Vector3.zero;
            return;
        }

        Vector3 playerPos = PlayerController.Instance.transform.position;
        playerPos.y = transform.position.y;

        Vector3 toPlayer = playerPos - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        // Switch to attack if close to player
        if(distanceToPlayer <= radiusOfSatisfaction)
        {
            EnterState(EnemyState.Attack);
            return;
        }

        // if we lost player go to idle
        if(distanceToPlayer > loseInterestRadius)
        {
            EnterState(EnemyState.Idle);
            return;
        }

        // A* Pathfinding
        bool targetMovedFar = (playerPos - lastPathTarget).sqrMagnitude >= repathDistanceThreshold * repathDistanceThreshold;

        bool needNewPath = pathPoints == null || pathPoints.Count == 0 || currentPathIndex >= pathPoints.Count || targetMovedFar || Time.time >= nextRepathTime;

        if (needNewPath)
        {
            //Try offset target
            Vector3 desiredTarget = playerPos + formationOffset;
            desiredTarget.y = transform.position.y;

            pathPoints = AStarPathFinding.FindPath(worldDecomposer, transform.position, desiredTarget);

            if (pathPoints == null || pathPoints.Count == 0)
            {
                pathPoints = AStarPathFinding.FindPath(worldDecomposer, transform.position, playerPos);
            }

            currentPathIndex = 0;
            lastPathTime = Time.time;
            lastPathTarget = playerPos;

            float jitter = Random.Range(-repathJitter, repathJitter);
            nextRepathTime = Time.time + Mathf.Max(0.05f, repathInterval + jitter);
        }

        // if still no path stand still
        if(pathPoints == null || pathPoints.Count == 0 || currentPathIndex >= pathPoints.Count)
        {
            pendingMove = Vector3.zero;
            return;
        }

        // Follow current waypoint
        Vector3 moveTarget = pathPoints[currentPathIndex];
        moveTarget.y = transform.position.y;

        Vector3 toWaypoint = moveTarget - transform.position;
        float distToWaypoint = toWaypoint.magnitude;

        // Move to next waypoint when close
        if(distToWaypoint <= radiusOfSatisfaction * 0.5f && currentPathIndex < pathPoints.Count - 1)
        {
            currentPathIndex++;
            moveTarget = pathPoints[currentPathIndex];
            moveTarget.y = transform.position.y;
            toWaypoint = moveTarget - transform.position;
        }

        Vector3 dir = toWaypoint.normalized;
        Vector3 separation = ComputeSeparation();

        Vector3 finalDir = dir + separation * separationStrength;

        if(finalDir.sqrMagnitude > 0.0001f)
        {
            finalDir.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(finalDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

            pendingMove = finalDir * moveSpeed * speedMultiplier;
        }
        else
        {
            pendingMove = Vector3.zero;
        }
    }

    private void UpdateAttack()
    {
        pendingMove = Vector3.zero;
        isAttacking = true;

        Vector3 playerPos = PlayerController.Instance.transform.position;
        playerPos.y = transform.position.y;

        Vector3 toPlayer = playerPos - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        // If player out of range Chase
        if (distanceToPlayer > radiusOfSatisfaction * 1.2f)
        {
            EnterState(EnemyState.Chase);
            return;
        }

        // Face player
        if (toPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toPlayer.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        // Let AttackPlayer handle the attack rate
        AttackPlayer();
    }

    // Animations
    private void UpdateAnimations()
    {
        if (animator == null) return;

        if (isDead)
        {
            animator.SetInteger("State", 2); // Death
            return;
        }

        // Needs idle animation
        switch (currentState)
        {
            case EnemyState.Attack:
                animator.SetInteger("State", 1); // Attack
                break;
            case EnemyState.Idle:
            case EnemyState.Alert:
            case EnemyState.Chase:
                animator.SetInteger("State", 0); // Walk / idle blend
                break;
        }
    }

    // Combat
    void AttackPlayer()
    {
        if (Time.time > lastAttackTime + attackRate)
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.TakeDamage(attackDamage);
            }
            lastAttackTime = Time.time;
        }
    }

    // ---------Public methods -----------
    

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0 || isDead) return;

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
        EnterState(EnemyState.Dead);

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.AddMoney(killReward);
        }

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

    // Helpers

    private Vector3 GetRandomFormationOffset()
    {
        // Pick a ring roughly around the melee radius
        float r = radiusOfSatisfaction * 0.8f;
        Vector2 offset2D = Random.insideUnitCircle.normalized;
        if (offset2D == Vector2.zero)
            offset2D = Vector2.right;

        return new Vector3(offset2D.x, 0f, offset2D.y) * r;
    }

    private Vector3 ComputeSeparation()
    {
        Vector3 separation = Vector3.zero;
        int neighborCount = 0;

        foreach (var other in AllEnemies)
        {
            if (other == null || other == this || other.isDead) continue;

            Vector3 toOther = transform.position - other.transform.position;
            float dist = toOther.magnitude;

            if (dist > 0f && dist < separationRadius)
            {
                separation += toOther.normalized / dist;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            separation /= neighborCount;
        }

        return separation;
    }

    private void OnDrawGizmosSelected()
    {
        if (pathPoints == null) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < pathPoints.Count; i++)
        {
            Gizmos.DrawSphere(pathPoints[i], 0.2f);
            if (i < pathPoints.Count - 1)
            {
                Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
            }
        }
    }
}