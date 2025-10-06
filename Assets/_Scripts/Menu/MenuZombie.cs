using UnityEngine;

public class MenuZombieIdle : MonoBehaviour
{
    private Animator anim;
    private float nextState;
    private float timer;

    private void Start()
    {
        anim = GetComponent<Animator>();
        PickNextState();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > nextState)
        {
            float randomState = Random.value;
            anim.SetFloat("State", randomState);
            PickNextState();
        }
    }

    void PickNextState()
    {
        nextState = Random.Range(2f, 6f);
        timer = 0f;
    }
}