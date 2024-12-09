using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
namespace FarmSystem
{
    public class CharacterPlayer : Character
    {

        PlayerControls playerControls;
        //Coroutines
        Coroutine C_Jump;
        Coroutine C_Roll;
        protected override void Awake()
        {
            base.Awake();
            playerControls = new PlayerControls();
            InputBinding();
        }
        protected override void Start()
        {
            base.Start();
            animator.runtimeAnimatorController = runTimeAnimatorController;
            speed = walkSpeed;
        }
        private void Update()
        {
            CheckGround();
        }
        private void FixedUpdate()
        {
            Move();
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
        //Hàm nhảy
        public void StartJump()
        {
            if (C_Jump == null && isGround)
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
            if (C_Roll == null && C_Jump == null && isGround)
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
        private void OnDrawGizmos()
        {
            // Vẽ Gizmos để dễ dàng kiểm tra hình tròn trong Scene
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1.0f);
        }
    }
}
