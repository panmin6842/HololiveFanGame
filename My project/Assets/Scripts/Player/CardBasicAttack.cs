using Mono.Cecil.Cil;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardBasicAttack : MonoBehaviour
{
    [SerializeField] private Transform cardStartPoint;
    [SerializeField] private Transform cardRightControlPoint;
    [SerializeField] private Transform cardLeftControlPoint;

    [SerializeField] private GameObject cam;

    [SerializeField] private GameObject cardPrefab;

    private PlayerControls inputActions;
    private Animator ani;
    private PlayerMovement playerMovement;

    private readonly int attack1Hash = Animator.StringToHash("Attack1");
    private readonly int attack2Hash = Animator.StringToHash("Attack2");

    private int comboCount = 0;
    private bool canCombo = false;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }
    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnComboOpen() => canCombo = true;
    public void OnComboClose()
    {
        canCombo = false;
        comboCount = 0;
    }

    private void Update()
    {
        Debug.Log(canCombo);
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (!canCombo && comboCount == 0)
        {
            playerMovement.noMove = true;
            ani.SetTrigger(attack1Hash);
            comboCount = 1;
        }
        else if (canCombo && comboCount == 1)
        {
            canCombo = false;
            playerMovement.noMove = true;
            ani.SetTrigger(attack2Hash);
            comboCount = 2;
        }
    }

    public void RightAttack() => LaunchCardProjectile(cardRightControlPoint.position);
    public void LeftAttack() => LaunchCardProjectile(cardLeftControlPoint.position);
    public void CanMove() => playerMovement.noMove = false;

    public void LaunchCardProjectile(Vector3 pos)
    {
        if (cardPrefab == null) return;

        Vector3 startPos = cardStartPoint.position;
        Vector3 controlPos = pos;
        Vector3 endPoint = startPos + (cam.transform.forward * 15f);

        GameObject newCard = Instantiate(cardPrefab, startPos, Quaternion.identity);

        if (newCard.TryGetComponent<CardMove>(out var cardMoveScript))
        {
            cardMoveScript.InitializeCurve(startPos, controlPos, endPoint);
        }
    }
}
