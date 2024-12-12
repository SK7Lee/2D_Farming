using UnityEngine;

namespace FarmSystem {
    public class AnimationEvent : MonoBehaviour
    {
        public Character owner;
        private void Awake()
        {
            owner = GetComponent<Character>();
        }
        public void Anim_OnActionBegin()
        {
            if (owner as CharacterAI)
            {
                (owner as CharacterAI).behaviorDecesion.currentBehaviorState = EBehaviorState.Executing;
            }
            owner.actionComponent.currentActionState = EActionState.Executing;
        }
        public void Anim_OnActionEnd()
        {
            owner.actionComponent.currentActionState = EActionState.Finish;
        }
    }
}