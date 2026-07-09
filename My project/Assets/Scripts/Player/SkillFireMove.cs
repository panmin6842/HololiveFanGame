using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SkillFireMove : MonoBehaviour
{
    public Vector3 attackPos;

    private CinemachineImpulseSource impulseSource;

    private float speed = 5f;
    private float nuckBackPower = 5f;

    [SerializeField] private GameObject hitParticle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //transform.LookAt(attackPos);
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, attackPos, Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            Rigidbody enemyRigid = other.GetComponent<Rigidbody>();
            Utils.NuckBackAndSpawnHitEffect(hitParticle, other, transform.position, enemyRigid, nuckBackPower);
            impulseSource.GenerateImpulse();
            Destroy(gameObject);
        }

        if(other.CompareTag("Ground"))
        {
            impulseSource.GenerateImpulse();
            Destroy(gameObject);
        }
    }
}
