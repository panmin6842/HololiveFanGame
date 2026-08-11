
using System.Collections;
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
    [SerializeField] private GameObject card;
    [SerializeField] private ParticleSystem cardParticle;

    [SerializeField] private Transform skillPos;
    [SerializeField] private GameObject skillReadyEffect;
    [SerializeField] private GameObject skillFire;

    private PlayerControls inputActions;
    private Animator ani;
    private PlayerMovement playerMovement;

    private readonly int attack1Hash = Animator.StringToHash("Attack1");
    private readonly int attack2Hash = Animator.StringToHash("Attack2");
    private readonly int skillHash = Animator.StringToHash("Skill");

    private int comboCount = 0;
    private bool canCombo = false;
    private bool skillStart = false;

    [SerializeField] private float detectRadius = 5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Collider[] detectEnemys;
    [SerializeField] private Collider closerEnemy;

    private float dist0;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }
    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Skill.performed += OnSkill;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Skill.performed -= OnSkill;
        inputActions.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        dist0 = 5;
    }

    public void OnComboOpen() => canCombo = true;
    public void OnComboClose()
    {
        canCombo = false;
        comboCount = 0;
    }

    private void DetectEnemies()
    {
        detectEnemys = Physics.OverlapSphere(transform.position, detectRadius, enemyLayer);

        if (detectEnemys.Length <= 0)
        {
            closerEnemy = null;
        }
        
        if (detectEnemys.Length > 0)
        {
            foreach (var enemyCollider in detectEnemys)
            {
                float dist = Vector3.Distance(transform.position, enemyCollider.transform.position);

                if (dist < dist0)
                {
                    dist0 = dist;
                    closerEnemy = enemyCollider;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }

    private void OnSkill(InputAction.CallbackContext context)
    {
        skillStart = true;
        playerMovement.noMove = true;
        DetectEnemies();

        cardParticle.Play();
        StartCoroutine(CardSetActiveFalse());
    }

    IEnumerator CardSetActiveFalse()
    {
        yield return new WaitForSeconds(0.1f);
        card.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        ani.SetTrigger(skillHash);
    }

    private GameObject newSkillReady;

    public void SkillReadyEffectSpawn()
    {
        newSkillReady = Instantiate(skillReadyEffect, skillPos.position, Quaternion.identity);
    }

    public void FireSpawn()
    {
        Vector3 enemyHitPoint = closerEnemy != null ? closerEnemy.gameObject.transform.Find("HitPoint").position
            : transform.position + new Vector3(0, 0, 5);
        Vector3 targetPos = enemyHitPoint != null ? enemyHitPoint : closerEnemy.gameObject.transform.position;

        Vector3 spawnDirection = (targetPos - skillPos.position).normalized;
        Quaternion spawnRotation = skillFire.transform.rotation;

        if(spawnDirection != Vector3.zero)
        {
            spawnRotation = Quaternion.LookRotation(-spawnDirection);
        }

        GameObject newFire = Instantiate(skillFire, skillPos.position, spawnRotation);
        SkillFireMove skillfireMove = newFire.GetComponent<SkillFireMove>();

        
        skillfireMove.attackPos = targetPos;
    }

    public void skillReset()
    {
        skillStart = false;
        card.SetActive(true);
        Destroy(newSkillReady);
        playerMovement.noMove = false;
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (!skillStart)
        {
            if (!canCombo && comboCount == 0)
            {
                DetectEnemies();
                playerMovement.noMove = true;
                ani.SetTrigger(attack1Hash);
                comboCount = 1;
            }
            else if (canCombo && comboCount == 1)
            {
                DetectEnemies();
                canCombo = false;
                playerMovement.noMove = true;
                ani.SetTrigger(attack2Hash);
                comboCount = 2;
            }
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
        Vector3 endPoint = closerEnemy != null ? closerEnemy.transform.position : transform.position + new Vector3(0, 0, 5);

        GameObject newCard = Instantiate(cardPrefab, startPos, Quaternion.identity);

        if (newCard.TryGetComponent<CardMove>(out var cardMoveScript))
        {
            cardMoveScript.InitializeCurve(startPos, controlPos, endPoint);
        }
    }
}
