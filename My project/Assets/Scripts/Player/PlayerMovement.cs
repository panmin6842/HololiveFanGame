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

    private CharacterController characterController;
    private PlayerControls inputActions;
    private Vector2 movementInput;
    private Vector3 moveDirection;

    private bool isSprintPressed;

    private readonly int speedHash = Animator.StringToHash("Speed");

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
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Player.Sprint.performed -= OnSprintPerformed;
        inputActions.Player.Sprint.canceled -= OnSprintCanceled;
        inputActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        RotatePlayer();
        UpdateAnimation();
    }

    private void OnMovePerformed(InputAction.CallbackContext context) => movementInput = context.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext context) => movementInput = Vector2.zero;

    private void OnSprintPerformed(InputAction.CallbackContext context) => isSprintPressed = true;
    private void OnSprintCanceled(InputAction.CallbackContext context) => isSprintPressed = false;

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

        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
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
