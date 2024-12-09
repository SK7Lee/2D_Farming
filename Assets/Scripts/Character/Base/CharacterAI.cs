using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace FarmSystem {
    [System.Serializable]
    public enum EMoveType
    {
        None = 0,
        Walk,
        Run
    }
    public class CharacterAI : Character
    {
        public BehaviorDecesion behaviorDecesion;

        [Header("NavMeshAgent")]
        public NavMeshAgent agent;
        protected Coroutine C_MoveToTarget;
        protected Coroutine C_Attack;
        protected Coroutine C_Roll;
        protected Coroutine C_Jump;
        protected override void Awake()
        {
            base.Awake();
            behaviorDecesion = GetComponent<BehaviorDecesion>();
        }

        protected override void Start()
        {
            base.Start();
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
        #region FuncCallOutSide
        public void StartJump()
        {
            if(C_Jump != null)
            {
                StopCoroutine(C_Jump);
            }
            C_Jump = StartCoroutine(Jump());
        }
        public void StartRoll(Vector2 rollDirection)
        {
            if (C_Roll != null)
            {
                StopCoroutine(C_Roll);
            }
            C_Roll = StartCoroutine(Roll(rollDirection));
        }
        public void StartMoveToTarget(Vector2 targetPosition, EMoveType moveType)
        {
            if (C_MoveToTarget != null)
            {
                StopCoroutine(C_MoveToTarget);
            }
            C_MoveToTarget = StartCoroutine(MoveToTarget(targetPosition, moveType));
        }
        #endregion
        #region Coroutines
        IEnumerator Jump()
        {
            float elapsedTime = 0.0f;
            //Start Jump
            animator.CrossFadeInFixedTime(AnimationParams.Jump_Start_State, .01f);
            behaviorDecesion.currentBehaviorState = EBehaviorState.Executing;
            while (elapsedTime < jumpDuration)
            {
                elapsedTime += Time.fixedDeltaTime;
                float raito = elapsedTime / jumpDuration;
                float factor = jumpCurve.Evaluate(raito);
                Vector2 displacement = new Vector2(0, factor * jumpHeight * Time.fixedDeltaTime);
                controller.Move(displacement);
                yield return null;
            }
            //End Jump
            behaviorDecesion.currentBehaviorState = EBehaviorState.Finish;
            animator.CrossFadeInFixedTime(AnimationParams.Jump_End_State, .01f);
            C_Jump = null;
        }
        IEnumerator Roll(Vector2 rolDirection)
        {
            float elapsedTime = 0.0f;
            //Start Roll
            animator.CrossFadeInFixedTime(AnimationParams.Roll_State, .01f);
            behaviorDecesion.currentBehaviorState = EBehaviorState.Executing;
            while (elapsedTime < rollDuration)
            {
                elapsedTime += Time.fixedDeltaTime;
                float raito = elapsedTime / rollDuration;
                float factor = rollCurve.Evaluate(raito);

                Vector2 displacement = rolDirection * factor * rollDistance * Time.fixedDeltaTime;
                controller.Move(displacement);
                yield return null;
            }
            //End Roll
            behaviorDecesion.currentBehaviorState = EBehaviorState.Finish;
            C_Roll = null;
        }
        IEnumerator MoveToTarget(Vector2 targetPosition, EMoveType moveType)
        {
            //Assign Factor
            float animBlendFactor = 0.0f;
            switch (moveType)
            {
                case EMoveType.Walk:
                    animBlendFactor = 2.0f;
                    speed = 3.5f;
                    break;
                case EMoveType.Run:
                    animBlendFactor = 6.0f;
                    speed = 5.0f;
                    break;
            }
            //Start Move
            agent.SetDestination(targetPosition);
            agent.speed = speed;
            animator.CrossFadeInFixedTime(AnimationParams.Speed_Param, animBlendFactor);
            behaviorDecesion.currentBehaviorState = EBehaviorState.Executing;
            while(Vector2.Distance(agent.transform.position, targetPosition) > agent.stoppingDistance)
            {
                Debug.Log("Executing Move");
                yield return null;
            }
            //End Move
            behaviorDecesion.currentBehaviorState = EBehaviorState.Finish;
            speed = 0.0f;
            animator.CrossFadeInFixedTime(AnimationParams.Speed_Param, 0.0f);
        }
        #endregion
    }
}
