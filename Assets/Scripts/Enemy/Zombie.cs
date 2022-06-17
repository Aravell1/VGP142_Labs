using UnityEngine;

public class Zombie : Enemy
{
    [SerializeField]
    public ParticleSystem rotBreath;

    int _health = 3;
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
    
    // Update is called once per frame
    void Update()
    {
        AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);

        if (curAnim[0].clip.name == "Dying")
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


            if (curAnim[0].clip.name == "Mutant Idle")
            {
                if (!rotBreath.isPlaying)
                    rotBreath.Play();
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
                if (rotBreath.isPlaying)
                    rotBreath.Stop();
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
                                
                if (Vector3.Distance(transform.position, Player.Instance.transform.position) < 5)
                {
                    transform.LookAt(Player.Instance.transform);
                    anim.SetBool("Attacking", true);
                }
                else
                    anim.SetBool("Attacking", false);
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
        if (curAnim[0].clip.name != "Dying" && collision.gameObject.tag == "PlayerProjectile")
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
        if ((curAnim[0].clip.name != "Dying" && other.gameObject.name == "KickCollider" && Player.Instance.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Mma Kick")
            || (other.gameObject.name == "PunchCollider" && Player.Instance.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cross Punch"))
        {
            SoundManager.Instance.Play(gotHitSound);
            health--;
        }
    }
}

