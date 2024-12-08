using UnityEngine;
using UnityEngine.AI;

public class CharacterAI : Character
{
    [Header("NavMeshAgent")]
    public NavMeshAgent agent;
    protected Coroutine  C_MoveToPosition;
    protected Coroutine C_Attack;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }


}
