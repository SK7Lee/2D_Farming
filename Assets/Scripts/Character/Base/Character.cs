using UnityEngine;

namespace FarmSystem
{
    public class Character : MonoBehaviour
    {
        [Header("Components")]
        public Animator animator;
        public AnimatorOverrideController runTimeAnimatorController;
        public CharacterController controller;
        public SpriteRenderer spriteRenderer;
        public TargetingComponent targetingComponent;
        public ActionComponent actionComponent;

        [Header("Movement OnGround")]
        [Tooltip("Idle - Walk - Run props")]
        public bool isSprint;
        public bool isFlip = false;
        public float speed = 0.0f;
        public float walkSpeed = 2.0f;
        public float runSpeed = 6.0f;
        public Vector2 moveDirection = Vector2.zero;
        public Vector2 lastMoveDirection = Vector2.zero;
        [Tooltip("Jump")]
        public float jumpDuration = .5f;
        public float jumpHeight = 5.0f;
        [Tooltip("Roll")]
        public float rollDuration = .5f;
        public float rollDistance = 5.0f;

        [Header("Movement OnWater")]
        public bool isGround = true;
        public LayerMask waterLayerMask;
        public float groundCheckRadius = 1.0f;
        public float groundCheckDistance = 1.0f;

        public float swimSpeed = 3.0f;
        public float fastSwimSpeed = 5.0f;

        [Header("Movement Variations")]
        public AnimationCurve jumpCurve;
        public AnimationCurve rollCurve;

        [Header("Current Tool (Test)")]
        public EToolType currentToolType = EToolType.Rod;

        public float energy = 100;
        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            targetingComponent = GetComponent<TargetingComponent>();
            targetingComponent = GetComponent<TargetingComponent>();
            actionComponent = GetComponent<ActionComponent>();
        }

        protected virtual void Start()
        {

        }
        //Hàm này lật Sprite nhân vật khi hướng di chuyển moveDirection.x thay đổi
        public void Flip()
        {
            if (moveDirection.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (moveDirection.x < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
        public void CheckGround()
        {
            // Thực hiện CircleCast
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, groundCheckRadius, Vector2.down, groundCheckDistance, waterLayerMask);

            if (hit.collider != null)
            {
                isGround = false;
                if (animator.GetBool(AnimationParams.IsGround_Param))
                {
                    animator.SetBool(AnimationParams.IsGround_Param, isGround);
                }
            }
            else
            {
                isGround = true;
                if (!animator.GetBool(AnimationParams.IsGround_Param))
                {
                    animator.SetBool(AnimationParams.IsGround_Param, isGround);
                }
            }
        }

    }
}