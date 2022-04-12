using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    CharacterController controller;

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
            controller.minMoveDistance = 0.0f;

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
    }

    // Update is called once per frame
    void Update()
    {
        /*float vInput = Input.GetAxis("Vertical");
        if (vInput > 0)
            transform.position += transform.forward * Time.deltaTime * speed;
        else if (vInput < 0)
            transform.position -= transform.forward * Time.deltaTime * speed;

        Vector3 moveRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);
        this.transform.Rotate(moveRotation);*/

        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
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
            Fire();
        }
    }

    void Fire()
    {
        Debug.Log("pew pew");
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

    private void OnCollisionEnter(Collider other)
    {
        if (other.gameObject.tag == "FinishPoint")
        {
            Debug.Log("LoadNextLevel");
            Debug.Log("Press Enter to end the game");
            GameManager.Instance.EndGameQuery();
        }
    }

    private void OnCollisionExit(Collider other)
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
