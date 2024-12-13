using FarmSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "Behavior Data", menuName = "Farm System/Behavior/Move Random Action")]
public class MoveRandomPosition : NormalAction
{
    public EMoveType MoveType;
    public float areaSize = 7.0f;
    public float step = 1.0f;

    public override void Execute(CharacterAI agent)
    {
        agent.StartMoveToTarget(agent.GetWalkableArea(agent.transform.position, areaSize, step), MoveType);
    }
    public override bool IsFinish(CharacterAI agent)
    {
        return base.IsFinish(agent);
    }
}
