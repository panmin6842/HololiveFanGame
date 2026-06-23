using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    private Animator ani;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 2f; //빠른 추락

    [SerializeField] private float groundedTimer;
    // 점프 유예시간(버퍼) 변수 추가
    private float groundedBufferTime = 0.15f;
    private float verticalVelocity; //Y축 속도

    private CharacterController characterController;
    private PlayerControls inputActions;
    private Vector2 movementInput;
    private Vector3 moveDirection;

    private bool isSprintPressed;

    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new PlayerControls();
        ani = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        inputActions.Player.Sprint.performed += OnSprintPerformed;
        inputActions.Player.Sprint.canceled += OnSprintCanceled;

        inputActions.Player.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Player.Sprint.performed -= OnSprintPerformed;
        inputActions.Player.Sprint.canceled -= OnSprintCanceled;

        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //매 프레임 바닥 체크 타이머
        if(characterController.isGrounded)
        {
            groundedTimer = groundedBufferTime;
        }
        else
        {
            groundedTimer -= Time.deltaTime;
        }

            CalculateMovement();
        RotatePlayer();
        UpdateAnimation();
    }

    private void OnMovePerformed(InputAction.CallbackContext context) => movementInput = context.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext context) => movementInput = Vector2.zero;

    private void OnSprintPerformed(InputAction.CallbackContext context) => isSprintPressed = true;
    private void OnSprintCanceled(InputAction.CallbackContext context) => isSprintPressed = false;

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if(groundedTimer > 0f)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * (gravity * gravityMultiplier));
            ani.SetTrigger(jumpHash);

            groundedTimer = 0;
        }
    }
    private void CalculateMovement()
    {
        // 카메라가 바라보는 방향
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        //이동 방향 계산
        moveDirection = (forward * movementInput.y) + (right * movementInput.x);

        float currentSpeed = (movementInput != Vector2.zero && isSprintPressed) ? runSpeed : walkSpeed;

        Vector3 finalMove = moveDirection * currentSpeed;

        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            // 공중에 떠 있다면 매 프레임 중력 적용
            verticalVelocity += gravity * gravityMultiplier * Time.deltaTime;
        }

        // 4. 평면 이동(X, Z)과 수직 이동(Y)을 결합
        finalMove.y = verticalVelocity;

        characterController.Move(finalMove * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        if(movementInput != Vector2.zero && moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateAnimation()
    {
        float targetAnimationSpeed = 0f;

        if (movementInput != Vector2.zero)
        {
            // 움직이고 있을 때 Shift를 누르면 1(뛰기), 안 누르면 0.5(걷기)
            targetAnimationSpeed = isSprintPressed ? 1f : 0.5f;
        }

        ani.SetFloat(speedHash, targetAnimationSpeed, 0.1f, Time.deltaTime);
    }
}
