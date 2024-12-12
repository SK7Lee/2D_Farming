using FarmSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "Behavior Data", menuName = "Farm System/Behavior/Move To Target Action")]
public class MoveToTarget : NormalAction
{
    public EMoveType MoveType;
    public override void Execute(CharacterAI agent)
    {
        agent.StartMoveToTarget(TargetPositionCalculate(agent), MoveType);
    }
    public override bool IsFinish(CharacterAI agent)
    {
        return base.IsFinish(agent);
    }
    public Vector2 TargetPositionCalculate(CharacterAI agent)
    {
        Vector2 targetPosition = Vector2.zero;
        if (agent.transform.position.x > agent.targetingComponent.target.transform.position.x)
        {
            targetPosition = agent.targetingComponent.target.position + Vector3.right * 1.5f;
        }
        else if (agent.transform.position.x < agent.targetingComponent.target.transform.position.x)
        {
            targetPosition = agent.targetingComponent.target.position - Vector3.right * 1.5f;
        }
        return targetPosition;
    }
}

