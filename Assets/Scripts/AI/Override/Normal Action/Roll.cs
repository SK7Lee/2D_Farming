using FarmSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "Behavior Data", menuName = "Farm System/Behavior/Roll Action")]
public class Roll : NormalAction
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

