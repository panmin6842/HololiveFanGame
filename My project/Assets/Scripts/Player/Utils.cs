using UnityEngine;

public static class Utils
{
    public static void NuckBackAndSpawnHitEffect(GameObject effectPrefab, Collider other, 
        Vector3 m_Pos, Rigidbody otherRigid, float nuckBackPower)
    {
        Vector3 point = other.ClosestPoint(m_Pos);
        Object.Instantiate(effectPrefab, point, Quaternion.identity);
        if (otherRigid != null)
        {
            Vector3 dist = other.transform.position - m_Pos;
            otherRigid.AddForce(dist * nuckBackPower, ForceMode.Impulse);
        }
    }
}
