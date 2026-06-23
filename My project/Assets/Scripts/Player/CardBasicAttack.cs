using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardBasicAttack : MonoBehaviour
{
    [SerializeField] private Transform cardStartPoint;
    [SerializeField] private Transform cardRightControlPoint;

    [SerializeField] private GameObject cam;

    [SerializeField] private GameObject cardPrefab;

    private PlayerControls inputActions;
    private Animator ani;

    public bool attack;

    private readonly int attack1Hash = Animator.StringToHash("Attack1");

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if(context.action.IsPressed())
        {
            attack = true;
            LaunchCardProjectile();
            ani.SetTrigger(attack1Hash);
        }
        
    }

    public void LaunchCardProjectile()
    {
        if (cardPrefab == null) return;

        Vector3 startPos = cardStartPoint.position;
        Vector3 controlPos = cardRightControlPoint.position;
        Vector3 endPoint = startPos + (cam.transform.forward * 15f);

        GameObject newCard = Instantiate(cardPrefab, startPos, Quaternion.identity);

        if (newCard.TryGetComponent<CardMove>(out var cardMoveScript))
        {
            cardMoveScript.InitializeCurve(startPos, controlPos, endPoint);
        }
    }
}
