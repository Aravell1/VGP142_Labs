using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    CharacterController controller;
    Fire playerFire;
    Animator anim;

    [Header("Player Settings")]
    [Space(2)]
    [Tooltip("Speed vaue between 1 and 6")]
    [Range(1.0f, 12.0f)]
    public float speed;
    public float storedSpeed;
    public float jumpSpeed;
    public float rotationSpeed;
    public float gravity;

    [Header("Weapon Settings")]
    public float projectileForce;
    public Rigidbody projectilePrefab;
    public Transform projectileSpawnPoint;

    Vector3 moveDirection;

    bool coroutineRunning = false;
    bool inWater = false;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            controller = GetComponent<CharacterController>();
            playerFire = GetComponent<Fire>();
            controller.minMoveDistance = 0.0f;
            anim = GetComponentInChildren<Animator>();

            if (speed <= 0)
            {
                speed = 8.0f;
            }
            if (jumpSpeed <= 0)
            {
                jumpSpeed = 6.0f;
            }
            if (rotationSpeed <= 0)
            {
                rotationSpeed = 10.0f;
            }
            if (gravity <= 0)
            {
                gravity = 9.81f;
            }
            moveDirection = Vector3.zero;
            if (projectileForce <= 0)
            {
                projectileForce = 10.0f;
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning(e.Message);
        }
        catch (UnassignedReferenceException e)
        {
            Debug.LogWarning(e.Message);
        }
        finally
        {
            Debug.Log("Always Gets Called");
        }

        //Debug.Log(anim.gameObject.name);

        storedSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);

        if (controller.isGrounded)
        {
            if (curAnim.Length > 0)
            {
                if (curAnim[0].clip.name != "Cross Punch" && curAnim[0].clip.name != "Mma Kick" && curAnim[0].clip.name != "Death")
                    moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                else
                    moveDirection = Vector3.zero;                
            }
            if (!inWater)
            {
                anim.SetFloat("Horizontal", moveDirection.x);
                anim.SetFloat("Vertical", moveDirection.z);
            }
            else
            {
                anim.SetFloat("Horizontal", moveDirection.x / 2);
                anim.SetFloat("Vertical", moveDirection.z / 2);
            }

            moveDirection *= speed;
            moveDirection = transform.TransformDirection(moveDirection);

            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpSpeed;
                anim.SetTrigger("Jump");
            }
        }
        /*else
        {
            moveDirection.x = Input.GetAxis("Horizontal") * speed;
            moveDirection.z = Input.GetAxis("Vertical") * speed;
        }*/       

        //Debug.Log(controller.isGrounded);
        //anim.SetBool("IsGrounded", controller.isGrounded);

        moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);

        if (!GameManager.Instance.pause && curAnim[0].clip.name != "Hit To Body" && Input.GetButtonDown("Fire1"))
            anim.SetTrigger("Attack");
        if (!GameManager.Instance.pause && curAnim[0].clip.name != "Hit To Body" && Input.GetButtonDown("Fire2"))
            anim.SetTrigger("Kick");
    }

    [ContextMenu("Reset Stats")]
    void ResetStats()
    {
        speed = 6.0f;
        jumpSpeed = 6.0f;
        rotationSpeed = 10.0f;
        gravity = 9.81f;
        projectileForce = 10.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);
        if ((curAnim[0].clip.name != "Hit To Body" && other.gameObject.name == "EnemyKickCollider" 
            && other.GetComponentInParent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Zombie Kicking" && curAnim[0].clip.name != "Death") 
            || (curAnim[0].clip.name != "Hit To Body" && other.gameObject.name == "EnemyPunchCollider" 
            && other.GetComponentInParent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Punching" && curAnim[0].clip.name != "Death"))
        {
            //Debug.Log("Got hit");
            GameManager.Instance.health--;
        }
        else if (other.gameObject.layer == 4)
        {
            speed = speed / 2;
            //Debug.Log("Hit Water");
            inWater = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);
        Debug.Log(other.gameObject.tag);
        Debug.Log(other.gameObject.name);
        if (curAnim[0].clip.name != "Hit To Body" && other.gameObject.tag == "EnemyProjectile" && curAnim[0].clip.name != "Death")
        {
            //Debug.Log("Got hit");
            Destroy(other.gameObject);
            GameManager.Instance.health--;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);
        if (curAnim[0].clip.name != "Hit To Body" && hit.gameObject.tag == "Mushroom")
        {
            Debug.Log("Mushroom");
            GameManager.Instance.health--;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            speed = speed * 2;
            //Debug.Log("Left Water");
            inWater = false;
        }
    }

    public void onPause()
    {
        if (GameManager.Instance.pause)
        {
            anim.speed = 0;
            storedSpeed = speed;
            speed = 0;
        }
        else
        {
            anim.speed = 1;
            speed = storedSpeed;
        }
    }
        
    public void StartJumpForceChange()
    {
        if (!coroutineRunning)
        {
            StartCoroutine("JumpForceChange");
        }
        else
        {
            StopCoroutine("JumpForceChange");
            jumpSpeed /= 2;
            StartCoroutine("JumpForceChange");
        }
    }

    IEnumerator JumpForceChange()
    {
        coroutineRunning = true;
        jumpSpeed *= 2;

        yield return new WaitForSeconds(5.0f);

        jumpSpeed /= 2;
        coroutineRunning = false;
    }

    public void StartSpeedForceChange()
    {
        if (!coroutineRunning)
        {
            StartCoroutine("SpeedForceChange");
        }
        else
        {
            StopCoroutine("SpeedForceChange");
            speed /= 1.5f;
            StartCoroutine("SpeedForceChange");
        }
    }

    IEnumerator SpeedForceChange()
    {
        coroutineRunning = true;
        speed *= 1.5f;

        yield return new WaitForSeconds(5.0f);

        speed /= 1.5f;
        coroutineRunning = false;
    }
}
