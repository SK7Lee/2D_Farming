using JetBrains.Annotations;
using UnityEngine;

namespace FarmSystem
{
    [System.Serializable]
    public enum EBehaviorCooldown
    {
        None = 0,
        Ready,
        WaitForCooldown
    }

    [System.Serializable]
    public abstract class SO_Behavior : ScriptableObject
    {
        public EBehaviorCooldown EBehaviorCooldown = EBehaviorCooldown.Ready;
        public float cooldown;
        public abstract void Execute(CharacterAI agent);
        public abstract bool IsFinish(CharacterAI agent);

    }
}
