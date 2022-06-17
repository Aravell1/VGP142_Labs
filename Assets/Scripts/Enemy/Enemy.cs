using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public Animator anim;
    public Rigidbody rb;
    public NavMeshAgent agent;   
    public GameObject target;
    public Slider healthBar;

    public AudioClip gotHitSound;
    public AudioClip deathSound;

    public enum EnemyType { Chase, Patrol };
    [SerializeField]
    public EnemyType enemyType;

    public GameObject[] path;
    public bool autoGenPath;
    public string pathName;
    public int pathIndex;

    public float distToNextNode;
    public float distToTarget;

    public float speed;

    public float timeOfLastFire;
    public float projectileFireRate;

    // Start is called before the first frame update
    public virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

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
                    RemoveElement(ref path, i);
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
            target = Player.Instance.gameObject;
        else if (enemyType == EnemyType.Patrol && path.Length > 0)
            target = path[pathIndex];

        if (target)
            agent.SetDestination(target.transform.position);

        speed = agent.speed;
    }

    private void RemoveElement<T>(ref T[] arr, int index)
    {
        for (int i = index; i < arr.Length - 1; i++)
            arr[i] = arr[i + 1];

        Array.Resize(ref arr, arr.Length - 1);
    }
}
