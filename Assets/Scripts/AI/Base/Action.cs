using JetBrains.Annotations;
using UnityEngine;

namespace FarmSystem
{
    public class Action : SO_Behavior
    {
        public override void Execute(CharacterAI agent) { }
        public override bool IsFinish(CharacterAI agent) { return false; }
    }
}
