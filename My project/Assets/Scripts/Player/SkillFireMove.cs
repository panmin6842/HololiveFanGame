using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SkillFireMove : MonoBehaviour
{
    public Vector3 attackPos;

    private CinemachineImpulseSource impulseSource;

    private float speed = 5f;

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
        if(other.CompareTag("Enemy") || other.CompareTag("Ground"))
        {
            impulseSource.GenerateImpulse();
            Destroy(gameObject);
        }
    }
}
