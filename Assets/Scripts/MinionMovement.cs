using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionMovement : MonoBehaviour
{
    [SerializeField]
    Transform Player;

    [SerializeField]
    Transform enemy;

    [SerializeField]
    Rigidbody projectilePrefab;

    [SerializeField]
    Transform projectileSpawnPoint;

    public LayerMask checkedLayers;

    public float speed;
    public float rotationSpeed;
    public float timeOfLastFire;
    public float projectileForce;
    public float projectileFireRate;

    // Start is called before the first frame update
    void Start()
    {
        if (speed <= 0)
        {
            speed = 3;
        }
        if (rotationSpeed <= 0)
        {
            rotationSpeed = 25;
        }
        if (projectileForce <= 0)
        {
            projectileForce = 75;
        }
        if (projectileFireRate <= 0)
        {
            projectileFireRate = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var towardsPlayer = Player.transform.position - transform.position;

        if (enemy.transform.gameObject.activeSelf == true && Vector3.Distance(transform.position, Player.position) >= 3)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(towardsPlayer), Time.deltaTime * rotationSpeed);
        }
        
        RaycastHit hitInfo;
        if (Physics.Raycast(projectileSpawnPoint.transform.position, transform.forward, out hitInfo, 20.0f, checkedLayers) && Time.time > timeOfLastFire + projectileFireRate)
        {
            Fire();
        }
        Vector3 endPos = transform.forward * 20.0f;
        Debug.DrawRay(projectileSpawnPoint.transform.position, endPos, Color.red);

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

    }

    public void EnableVisibility()
    {
        enemy.transform.gameObject.SetActive(true);
    }

    public void DisableVisibility()
    {
        enemy.transform.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enemy.transform.gameObject.activeSelf == false)
        {
            Debug.Log("Should ignore collision");
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }

    public void Fire()
    {
        timeOfLastFire = Time.time;

        Rigidbody temp;
        temp = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        temp.gameObject.GetComponent<Rigidbody>().velocity = transform.forward * projectileForce;
    }
}
