using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
public class MinionMovement : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;
    NavMeshAgent agent;

    public Pickups[] pickupsPrefabArray;
    private GameObject Player;
    public GameObject target;

    [SerializeField]
    Transform enemy;

    [SerializeField]
    Transform rayOrigin;

    enum EnemyType { Chase, Patrol };
    [SerializeField]
    EnemyType enemyType;

    public GameObject[] path;
    public bool autoGenPath;
    public string pathName;
    public int pathIndex;
    
    public float distToNextNode;
    public float distToTarget;
    
    private float speed;

    public float timeOfLastFire;
    public float projectileFireRate;

    public Slider healthBar;
    int _health = 2;
    public int health
    {
        get
        {
            return _health;
        }
        set
        {
            if (_health > value)
            {
                healthBar.value = value;
            }

            _health = value;

            if (_health <= 0)
            {
                anim.SetTrigger("Death");
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        if (projectileFireRate <= 0)
        {
            projectileFireRate = 10;
        }

        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        if (string.IsNullOrEmpty(pathName))
            pathName = "PatrolNode";

        if (distToNextNode <= 0)
            distToNextNode = 1.0f;

        if (autoGenPath)
        {
            path = GameObject.FindGameObjectsWithTag(pathName);

            for (int i = 0; i < path.Length; i++)
            {
                if (Vector3.Distance(transform.position, path[i].transform.position) > 40)
                {
                    RemoveElement(ref path, i);
                }
            }

            if (path.Length > 0)
            {
                for (int i = 0; i < path.Length; i++)
                {
                    GameObject temp = path[i];
                    int rand = UnityEngine.Random.Range(0, path.Length);
                    path[i] = path[rand];
                    path[rand] = temp;
                }
            }
        }

        if (!target && enemyType == EnemyType.Chase)
            target = Player;
        else if (enemyType == EnemyType.Patrol && path.Length > 0)
            target = path[pathIndex];

        if (target)
            agent.SetDestination(target.transform.position);

        speed = agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);
        if (curAnim[0].clip.name != "Zombie Death")
        {
            distToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (target && enemyType == EnemyType.Patrol)
            {
                if (agent.remainingDistance < distToNextNode)
                {
                    if (path.Length > 0)
                    {
                        pathIndex++;
                        pathIndex %= path.Length;
                        if (pathIndex == 0)
                        {
                            for (int i = 0; i < path.Length; i++)
                            {
                                GameObject temp = path[i];
                                int rand = UnityEngine.Random.Range(0, path.Length);
                                path[i] = path[rand];
                                path[rand] = temp;
                            }
                        }

                        target = path[pathIndex];
                    }
                }
            }

            if (target)
                agent.SetDestination(target.transform.position);


            if (curAnim[0].clip.name == "Zombie Kicking" || curAnim[0].clip.name == "Punching")
            {
                anim.SetFloat("Speed", 0);
                agent.speed = 0;
            }
            else if (curAnim[0].clip.name == "Throw Object")
            {
                anim.SetFloat("Speed", 0);
                agent.speed = 0;
                transform.LookAt(Player.transform);
            }
            else
            {
                anim.SetFloat("Speed", 1);
                agent.speed = speed;
            }
        
            if (Vector3.Distance(transform.position, Player.transform.position) < 25)
            {
                if (enemyType == EnemyType.Patrol)
                {
                    enemyType = EnemyType.Chase;
                    target = Player;
                    agent.SetDestination(target.transform.position);
                }

                if (Time.time > timeOfLastFire + projectileFireRate && Vector3.Distance(transform.position, Player.transform.position) >= 4)
                {
                    timeOfLastFire = Time.time;
                    anim.SetTrigger("Throw");
                }
                else if (Time.time > timeOfLastFire + projectileFireRate && Vector3.Distance(transform.position, Player.transform.position) < 4)
                {
                    //Debug.Log("Melee Attack");
                    timeOfLastFire = Time.time;
                    transform.LookAt(Player.transform);
                    int randomTemp = UnityEngine.Random.Range(0, 2);
                    if (randomTemp == 0)
                        anim.SetTrigger("Punch");
                    else if (randomTemp == 1)
                        anim.SetTrigger("Kick");
                }
            }
            else if (Vector3.Distance(transform.position, Player.transform.position) > 30 && enemyType == EnemyType.Chase)
            {
                enemyType = EnemyType.Patrol;
                target = path[pathIndex];
                agent.SetDestination(target.transform.position);
            }

            if (GameManager.Instance.pause)
            {
                anim.speed = 0;
                agent.speed = 0;
            }
            else
            {
                anim.speed = 1;
                agent.speed = speed;
            }
        }

        healthBar.transform.LookAt(GameObject.Find("Main Camera").transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collided with " + collision.gameObject.name);
        if (collision.gameObject.tag == "PlayerProjectile")
        {
            Destroy(collision.gameObject);
            health--;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided with " + other.gameObject.name);
        if ((other.gameObject.name == "KickCollider" && Player.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Mma Kick")
            || (other.gameObject.name == "PunchCollider" && Player.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cross Punch"))
        {
            health--;
        }
    }

    private void RemoveElement<T>(ref T[] arr, int index)
    {
        for (int i = index; i < arr.Length - 1; i++)
            arr[i] = arr[i + 1];

        Array.Resize(ref arr, arr.Length - 1);
    }
}
