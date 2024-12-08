using UnityEngine;

public class CharacterEnemy : CharacterAI
{
    //Testing
    public Transform target;
    public float timeDelay = 1.0f;
    public float timeUpdated = 0.0f;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }

    //Testing
    private void Update()
    {
        //Testing
        if(Time.time > timeUpdated + timeDelay) {
            agent.SetDestination(target.position);
            timeUpdated = Time.time;
        }

        CheckGround();
    }
}
