using UnityEngine;
namespace FarmSystem
{
    public class CharacterEnemy : CharacterAI
    {
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            animator.runtimeAnimatorController = runTimeAnimatorController;
            base.Start();

        }
        private void Update()
        {
            CheckGround();
        }
        private void FixedUpdate()
        {
        }
    }
}