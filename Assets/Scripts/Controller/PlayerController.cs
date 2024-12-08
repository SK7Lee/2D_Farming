using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using static UnityEngine.UI.Image;


public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public AnimatorOverrideController runTimeAnimatorController;
    public CharacterController controller;
    public SpriteRenderer spriteRenderer;

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

    //Coroutines
    Coroutine C_Jump;
    Coroutine C_Roll;
    //Input System
    PlayerControls playerControls;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        InputBinding();
    }
    private void Start()
    {
        animator.runtimeAnimatorController = runTimeAnimatorController;
        speed = walkSpeed;
    }

    private void FixedUpdate()
    {
        Move();
        CheckGround();
    }
    #region InputManager
    private void InputBinding()
    {
        playerControls.Movement.Move.performed += Move;
        playerControls.Movement.Move.canceled += MoveReleased;
        playerControls.Movement.Sprint.performed += PressedSprint;
        playerControls.Movement.Sprint.canceled += ReleasedSprint;
        playerControls.Movement.Jump.performed += Jump;
        playerControls.Movement.Roll.performed += Roll;
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    private void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveDirection = context.ReadValue<Vector2>();
            Flip();
            moveDirection.Normalize();
        }
    }
    private void MoveReleased(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            moveDirection = Vector2.zero;
        }
    }
    private void PressedSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprint = true;
        }
    }
    private void ReleasedSprint(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            isSprint = false;
        }
    }
    private void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartJump();
        }
    }
    private void Roll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartRoll();
        }
    }
    #endregion
    #region MovementManager
    //Hàm di chuyển nhân vật
    public void Move()
    {
        if (isGround)
        {
            if (moveDirection.magnitude > 0)
            {
                float targetSpeed = isSprint ? runSpeed : walkSpeed;
                speed = targetSpeed;
                animator.SetFloat(AnimationParams.Speed_Param, speed);
                controller.Move(moveDirection * targetSpeed * Time.fixedDeltaTime);
            }
            else
            {
                speed = 0.0f;
                animator.SetFloat(AnimationParams.Speed_Param, speed);
            }
        }
        else
        {
            if (moveDirection.magnitude > 0)
            {
                float targetSpeed = isSprint ? fastSwimSpeed : swimSpeed;
                controller.Move(moveDirection * targetSpeed * Time.fixedDeltaTime);
            }

        }
    }
    //Hàm này lật Sprite nhân vật khi hướng di chuyển moveDirection.x thay đổi
    public void Flip()
    {
        if(moveDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if(moveDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
    //Hàm nhảy
    public void StartJump()
    {
        if(C_Jump == null && isGround)
            C_Jump = StartCoroutine(Jump());
    }
    IEnumerator Jump()
    {
        float elapsedTime = 0.0f;
        animator.CrossFadeInFixedTime(AnimationParams.Jump_Start_State, .01f);
        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.fixedDeltaTime;
            float raito = elapsedTime / jumpDuration;
            float factor = jumpCurve.Evaluate(raito);
            Vector2 displacement = new Vector2(0, factor * jumpHeight * Time.fixedDeltaTime);
            controller.Move(displacement);

            //if (raito > .8f && count == 0)
            //{
            //    animator.CrossFadeInFixedTime("Jump End", .01f);
            //    count++;
            //}
            yield return null;
        }
        animator.CrossFadeInFixedTime(AnimationParams.Jump_End_State, .01f);
        C_Jump = null;
    }
    //Hàm lăn nhân vật
    public void StartRoll()
    {
        if(C_Roll == null && C_Jump == null && isGround) 
        C_Roll = StartCoroutine(Roll());
    }
    IEnumerator Roll()
    {
        float elapsedTime = 0.0f;
        animator.CrossFadeInFixedTime(AnimationParams.Roll_State, .01f);
        while (elapsedTime < rollDuration)
        {
            elapsedTime += Time.fixedDeltaTime;
            float raito = elapsedTime / rollDuration;
            float factor = rollCurve.Evaluate(raito);

            Vector2 displacement = moveDirection * factor * rollDistance * Time.fixedDeltaTime;
            controller.Move(displacement);
            yield return null;
        }
        C_Roll = null;
    }
    #endregion
    
    void CheckGround()
    {
        // Thực hiện CircleCast
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, groundCheckRadius, Vector2.down, groundCheckDistance, waterLayerMask);

        if (hit.collider != null)
        {
            isGround = false;
            if (animator.GetBool(AnimationParams.IsGround_Param))
            {
                Debug.Log("Set To False");
                animator.SetBool(AnimationParams.IsGround_Param, isGround);
            }
        }
        else
        {
            isGround = true;
            if (!animator.GetBool(AnimationParams.IsGround_Param))
            {
                Debug.Log("Set To True");
                animator.SetBool(AnimationParams.IsGround_Param, isGround);
            }
        }
    }
    private void OnDrawGizmos()
    {
        // Vẽ Gizmos để dễ dàng kiểm tra hình tròn trong Scene
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.0f);
    }

}
