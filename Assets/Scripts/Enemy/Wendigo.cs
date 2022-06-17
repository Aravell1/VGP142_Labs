using UnityEngine;

public class Wendigo : Enemy
{
    int _health = 1;
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
            projectileFireRate = 8;
        }
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);

        if (curAnim[0].clip.name == "Dying Backwards")
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            agent.enabled = false;
        }
        else
        {
            if (target)
                distToTarget = Vector3.Distance(transform.position, target.transform.position);
            else if (enemyType == EnemyType.Patrol && !target)
                target = path[pathIndex];

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
            {
                healthBar.gameObject.SetActive(true);
                transform.LookAt(Player.Instance.transform);
            }

            if (target && agent.enabled && enemyType == EnemyType.Patrol)
                agent.SetDestination(target.transform.position);


            if (curAnim[0].clip.name == "Two Hand Spell Casting")
            {
                if (agent.enabled)
                {
                    //agent.speed = 0;
                    agent.enabled = false;
                }
                anim.SetFloat("Speed", 0);
                transform.LookAt(Player.Instance.transform);
            }
            else if (enemyType == EnemyType.Patrol)
            {
                if (!agent.enabled)
                {
                    //agent.speed = speed;
                    agent.enabled = true;
                }
                anim.SetFloat("Speed", 1);
            }

            if (Vector3.Distance(transform.position, Player.Instance.transform.position) <= 30)
            {
                if (enemyType == EnemyType.Patrol)
                {
                    enemyType = EnemyType.Chase;
                    anim.SetTrigger("Attack");
                }
                if (agent.enabled)
                    agent.enabled = false;

                anim.SetFloat("Speed", 0);

                if (Time.time > timeOfLastFire + projectileFireRate)
                {
                    timeOfLastFire = Time.time;
                    anim.SetTrigger("Attack");
                }
            }
            else if (Vector3.Distance(transform.position, Player.Instance.transform.position) > 30 && Vector3.Distance(transform.position, Player.Instance.transform.position) < 60 && enemyType == EnemyType.Chase)
            {
                agent.enabled = true;
                target = Player.Instance.gameObject;
                agent.SetDestination(target.transform.position);
                anim.SetFloat("Speed", 1);
            }
            else if (Vector3.Distance(transform.position, Player.Instance.transform.position) >= 60 && enemyType == EnemyType.Chase)
            {
                enemyType = EnemyType.Patrol;
                target = path[pathIndex];
                agent.enabled = true;
                agent.speed = speed;
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
        if (curAnim[0].clip.name != "Dying Backwards" && collision.gameObject.tag == "PlayerProjectile")
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
        if ((curAnim[0].clip.name != "Dying Backwards" && other.gameObject.name == "KickCollider" && Player.Instance.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Mma Kick")
            || (other.gameObject.name == "PunchCollider" && Player.Instance.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cross Punch"))
        {
            SoundManager.Instance.Play(gotHitSound);
            health--;
        }
    }
}

