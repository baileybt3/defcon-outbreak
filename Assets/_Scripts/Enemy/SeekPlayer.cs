using UnityEngine;


public class SeekPlayer : MonoBehaviour
{
    // Movement Constants
    [SerializeField] private float moveSpeed;
    private float radiusOfSatisfaction;
    private float slowRadius;

    [SerializeField] private Transform myPlayer;
    [SerializeField] private Transform myEnemy;

    //Movement States
    private Vector3 targetPosition;
    private bool hasTarget = false;

    private void Start()
    {
        moveSpeed = 2f;
        radiusOfSatisfaction = 1f;
        slowRadius = 5f;
    }

    void Update()
    {
        if (myPlayer != null)
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
    
        Vector3 towardsTarget = myPlayer.position - myEnemy.position;
        float distance = towardsTarget.magnitude;

        if (distance <= radiusOfSatisfaction)
        {
            return;
        }

        towardsTarget = towardsTarget.normalized;

        Quaternion targetRotation = Quaternion.LookRotation(towardsTarget);
        myEnemy.rotation = Quaternion.Lerp(myEnemy.rotation, targetRotation, 0.1f);

        float targetSpeed = moveSpeed;
        if (distance < slowRadius)
        {
            targetSpeed = moveSpeed * (distance / slowRadius);
        }

        myEnemy.position += myEnemy.forward * targetSpeed * Time.deltaTime;

    }
}
