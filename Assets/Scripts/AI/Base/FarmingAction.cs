using FarmSystem;
using UnityEngine;

public class FarmingAction : Action
{
    public override void Execute(CharacterAI agent)
    {
        base.Execute(agent);
    }
    public override bool IsFinish(CharacterAI agent)
    {
        return base.IsFinish(agent);
    }
}
