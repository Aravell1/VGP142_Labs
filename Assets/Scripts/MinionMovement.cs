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
    Transform rayOrigin;

    public LayerMask checkedLayers;

    public float speed;
    public float rotationSpeed;
    public float timeOfLastFire;
    public float projectileFireRate;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (speed <= 0)
        {
            speed = 3;
        }
        if (rotationSpeed <= 0)
        {
            rotationSpeed = 25;
        }
        if (projectileFireRate <= 0)
        {
            projectileFireRate = 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var towardsPlayer = Player.transform.position - transform.position;
        AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);

        if (enemy.transform.gameObject.activeSelf == true && Vector3.Distance(transform.position, Player.position) >= 3)
        {
            if (curAnim[0].clip.name != "Throw Object" && curAnim[0].clip.name != "Zombie Kicking" && curAnim[0].clip.name != "Punching")
            { 
                transform.position += transform.forward * speed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(towardsPlayer), Time.deltaTime * rotationSpeed);
                anim.SetFloat("Speed", 1);
            }
            else
                anim.SetFloat("Speed", 0);
        }
        
        RaycastHit hitInfo;
        if (Physics.Raycast(rayOrigin.transform.position, transform.forward, out hitInfo, 20.0f, checkedLayers) && Time.time > timeOfLastFire + projectileFireRate)
        {
            timeOfLastFire = Time.time;

            if (Vector3.Distance(transform.position, Player.position) >= 5)
                anim.SetTrigger("Throw");
            else
            {
                int randomTemp = Random.Range(0, 2);
                if (randomTemp == 0)
                    anim.SetTrigger("Punch");
                else if (randomTemp == 1)
                    anim.SetTrigger("Kick");
            }

        }
        Vector3 endPos = transform.forward * 20.0f;
        Debug.DrawRay(rayOrigin.transform.position, endPos, Color.red);

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
        Debug.Log("Collided with " + collision.gameObject.name);
        if (enemy.transform.gameObject.activeSelf == false)
        {
            Debug.Log("Should ignore collision");
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
        else if (collision.gameObject.tag == "PlayerProjectile")
        {
            Debug.Log("Should Die");
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.gameObject.name);
        if (other.gameObject.name == "KickCollider" && Player.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Mma Kick")
        {
            Debug.Log("Should Die");
            Destroy(gameObject);
        }
        else if (other.gameObject.name == "PunchCollider" && Player.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cross Punch")
        {
            Debug.Log("Should Die");
            Destroy(gameObject);
        }
    }


}
