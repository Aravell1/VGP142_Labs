using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    [SerializeField]
    Rigidbody projectilePrefab;

    [SerializeField]
    Transform projectileSpawnPoint;

    [SerializeField]
    GameObject parentEnemy;

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
        temp.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(dirToPlayer.x, dirToPlayer.y + 0.2f, dirToPlayer.z) * projectileForce;
        Destroy(temp.gameObject, 5.0f);
    }

    public void Death()
    {
        Instantiate(GetComponentInParent<MinionMovement>().pickupsPrefabArray[UnityEngine.Random.Range(0, GetComponentInParent<MinionMovement>().pickupsPrefabArray.Length)],
            new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z), Quaternion.Euler(90, transform.rotation.y, 0));
        GameManager.Instance.score += 10;
        Destroy(parentEnemy);
    }
}
