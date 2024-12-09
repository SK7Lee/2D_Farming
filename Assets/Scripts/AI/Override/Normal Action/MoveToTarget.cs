using FarmSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "Behavior Data", menuName = "Farm System/Behavior/Move To Target Action")]
public class MoveToTarget : NormalAction
{
    public EMoveType MoveType;
    public override void Execute(CharacterAI agent)
    {
        agent.StartMoveToTarget(agent.targetingComponent.target.position, MoveType);
    }
    public override bool IsFinish(CharacterAI agent)
    {
        return base.IsFinish(agent);
    }
}

