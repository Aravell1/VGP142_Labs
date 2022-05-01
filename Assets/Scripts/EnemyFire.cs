using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    [SerializeField]
    Rigidbody projectilePrefab;

    [SerializeField]
    Transform projectileSpawnPoint;

    public float projectileForce;

    private void Start()
    {
        if (projectileForce <= 0)
        {
            projectileForce = 30;
        }
    }

    public void Fire()
    {
        Rigidbody temp;
        temp = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        Vector3 dirToPlayer = (GameObject.Find("Player").transform.position - temp.transform.position).normalized;
        temp.gameObject.GetComponent<Rigidbody>().velocity = dirToPlayer * projectileForce;
        Destroy(temp.gameObject, 2.0f);
    }
}
