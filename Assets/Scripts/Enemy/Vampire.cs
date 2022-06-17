using UnityEngine;

public class Vampire : Enemy
{
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
                SoundManager.Instance.Play(deathSound);
                anim.SetTrigger("Death");
            }
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if (projectileFireRate <= 0)
        {
            projectileFireRate = 6;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);

        if (curAnim[0].clip.name == "Zombie Death")
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            agent.enabled = false;
        }
        else
        {
            if (target)
                distToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (target && enemyType == EnemyType.Patrol)
            {
                healthBar.gameObject.SetActive(false);
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
                                int rand = Random.Range(0, path.Length);
                                path[i] = path[rand];
                                path[rand] = temp;
                            }
                        }

                        target = path[pathIndex];
                    }
                }
            }
            else if (enemyType == EnemyType.Chase)
                healthBar.gameObject.SetActive(true);

            if (target && agent.enabled)
                agent.SetDestination(target.transform.position);


            if (curAnim[0].clip.name == "Zombie Kicking" || curAnim[0].clip.name == "Punching")
            {
                if (agent.enabled)
                {
                    anim.SetFloat("Speed", 0);
                    agent.speed = 0;
                    agent.enabled = false;
                }
            }
            else if (curAnim[0].clip.name == "Throw Object")
            {
                if (agent.enabled)
                {
                    anim.SetFloat("Speed", 0);
                    agent.speed = 0;
                    agent.enabled = false;
                }
                transform.LookAt(Player.Instance.transform);
            }
            else
            {
                if (!agent.enabled)
                {
                    agent.speed = speed;
                    anim.SetFloat("Speed", 1);
                    agent.enabled = true;
                }
            }
        
            if (Vector3.Distance(transform.position, Player.Instance.transform.position) < 25)
            {
                if (enemyType == EnemyType.Patrol)
                {
                    enemyType = EnemyType.Chase;
                    target = Player.Instance.gameObject;
                    agent.SetDestination(target.transform.position);
                }

                if (Time.time > timeOfLastFire + projectileFireRate && Vector3.Distance(transform.position, Player.Instance.transform.position) >= 2)
                {
                    timeOfLastFire = Time.time;
                    anim.SetTrigger("Throw");
                }
                else if (Time.time > timeOfLastFire + projectileFireRate && Vector3.Distance(transform.position, Player.Instance.transform.position) < 2)
                {
                    //Debug.Log("Melee Attack");
                    timeOfLastFire = Time.time;
                    transform.LookAt(Player.Instance.transform);
                    int randomTemp = Random.Range(0, 2);
                    if (randomTemp == 0)
                        anim.SetTrigger("Punch");
                    else if (randomTemp == 1)
                        anim.SetTrigger("Kick");
                }
            }
            else if (Vector3.Distance(transform.position, Player.Instance.transform.position) > 30 && enemyType == EnemyType.Chase && agent.enabled)
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
        AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);
        //Debug.Log("Collided with " + collision.gameObject.name);
        if (curAnim[0].clip.name != "Zombie Death" && collision.gameObject.tag == "PlayerProjectile")
        {
            SoundManager.Instance.Play(gotHitSound);
            Destroy(collision.gameObject);
            health--;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);
        //Debug.Log("Collided with " + other.gameObject.name);
        if ((curAnim[0].clip.name != "Zombie Death" && other.gameObject.name == "KickCollider" && Player.Instance.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Mma Kick")
            || (other.gameObject.name == "PunchCollider" && Player.Instance.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cross Punch"))
        {
            SoundManager.Instance.Play(gotHitSound);
            health--;
        }
    }

    
}
