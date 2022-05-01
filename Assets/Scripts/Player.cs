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
    [Range(1.0f, 6.0f)]
    public float speed;
    public float jumpSpeed;
    public float rotationSpeed;
    public float gravity;

    [Header("Weapon Settings")]
    public float projectileForce;
    public Rigidbody projectilePrefab;
    public Transform projectileSpawnPoint;

    Vector3 moveDirection;

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
                speed = 6.0f;
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

            /*if (!projectilePrefab)
            {
                throw new UnassignedReferenceException("Projectile Prefab is unassigned");
            }*/
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

        Debug.Log(anim.gameObject.name);
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
            anim.SetFloat("Horizontal", moveDirection.x);
            anim.SetFloat("Vertical", moveDirection.z);

            moveDirection *= speed;
            moveDirection = transform.TransformDirection(moveDirection);

            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        /*else
        {
            moveDirection.x = Input.GetAxis("Horizontal") * speed;
            moveDirection.z = Input.GetAxis("Vertical") * speed;
        }*/

        moveDirection.y -= gravity * Time.deltaTime;


        controller.Move(moveDirection * Time.deltaTime);

        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("Attack");
        }
        if (Input.GetButtonDown("Fire2"))
        {
            anim.SetTrigger("Kick");
        }
    }

    /*void Fire()
    {
        Debug.Log("pew pew");
        playerFire.FireProjectile();
    }*/

    [ContextMenu("Reset Stats")]
    void ResetStats()
    {
        speed = 6.0f;
        jumpSpeed = 6.0f;
        rotationSpeed = 10.0f;
        gravity = 9.81f;
        projectileForce = 10.0f;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "FinishPoint")
        {
            Debug.Log("LoadNextLevel");
            Debug.Log("Press Enter to end the game");
            GameManager.Instance.EndGameQuery();
        }        
        else if (other.gameObject.tag == "EnemyProjectile")
        {
            Debug.Log("Got hit");
            anim.SetTrigger("Death");
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "FinishPoint")
        {
            GameManager.Instance.EndGameQuery();
        }
    }

    /*private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "FinishPoint")
        {
            Debug.Log("LoadNextLevel");
            Debug.Log("Press Enter to end the game");
            GameManager.Instance.EndGameQuery();
        }
    }*/
}
