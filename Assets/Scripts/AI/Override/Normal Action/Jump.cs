using FarmSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "Behavior Data", menuName = "Farm System/Behavior/Jump Action")]
public class Jump : NormalAction
{
    public override void Execute(CharacterAI agent)
    {
        agent.StartJump();
    }
    public override bool IsFinish(CharacterAI agent)
    {
        return base.IsFinish(agent);
    }

}
