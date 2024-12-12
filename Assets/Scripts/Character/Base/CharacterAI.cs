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
    public enum EFlipType
    {
        None = 0,
        AgentVelocity,
        ActionExecute
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

        [Header("Current Target Tree")]
        public string treeTargetName;
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
        public void Flip(EFlipType flipType)
        {
            switch (flipType)
            {
                case EFlipType.AgentVelocity:
                    Vector3 moveDirection = agent.velocity.normalized;
                    if (moveDirection.x > 0)
                    {
                        spriteRenderer.flipX = false;
                    }
                    else if (moveDirection.x < 0)
                    {
                        spriteRenderer.flipX = true;
                    }
                    break;
                case EFlipType.ActionExecute:
                    Vector2 targetPosition = targetingComponent.target.transform.position;
                    if(targetPosition.x > gameObject.transform.position.x)
                    {
                        spriteRenderer.flipX = false;
                    }
                    else if(targetPosition.x < gameObject.transform.position.x)
                    {
                        spriteRenderer.flipX = true;
                    }
                    break;
                default:
                    break;
            }

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
            animator.CrossFadeInFixedTime(AnimationParams.Locomotion_State, .1f);
            animator.SetFloat(AnimationParams.Speed_Param, animBlendFactor);
            //Disable avoid Agent
            //agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            //Start Move
            agent.SetDestination(targetPosition);
            agent.speed = speed;
            behaviorDecesion.currentBehaviorState = EBehaviorState.Executing;
            while (!HasReachedDestination())
            {
                if (targetingComponent.isCurrentSoilTargetInProcess()) break;
                Debug.Log("Executing Move " + gameObject.name);
                Flip(EFlipType.AgentVelocity);
                yield return null;
            }
            //agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            behaviorDecesion.currentBehaviorState = EBehaviorState.Finish;
            speed = 0.0f;
            animator.SetFloat(AnimationParams.Speed_Param, 0.0f);
        }

        bool HasReachedDestination()
        {
            // Kiểm tra nếu agent hoặc đích không hợp lệ
            if (agent == null || targetingComponent.target == null)
            {
                return false;
            }

            // Kiểm tra nếu agent đã dừng cách mục tiêu trong khoảng stoppingDistance
            if (!agent.pathPending) // Đảm bảo agent đã tính xong đường đi
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) // Kiểm tra nếu agent đã dừng
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    #endregion
}

