using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class Player : Singleton<Player>
{
    CharacterController controller;
    Animator anim;
    Vector3 moveDirection;

    [Header("Player Settings")]
    [Space(2)]
    [Tooltip("Speed vaue between 1 and 6")]
    [Range(1.0f, 12.0f)]
    public float speed;
    public float storedSpeed;
    public float jumpSpeed;
    public float rotationSpeed;
    public float gravity;

    [Header("Cameras")]
    public Camera fpsCam;
    public Camera mainCam;
    
    [Header("Pickup Settings")]
    public float jumpModeTimer = 5;
    public float jumpMultiplier = 1.5f;
    public GameObject gunContainer;

    [Header("Field Effect Settings")]
    bool inWater = false;

    private float rotTimer = 6;
    private float healTimer = 0;

    [Header("Audio Clips")]
    public AudioClip healSound;
    public AudioClip gotHitSound;
    public AudioClip splashSound; 
    public AudioClip deathSound;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            controller = GetComponent<CharacterController>();
            controller.minMoveDistance = 0.0f;
            anim = GetComponentInChildren<Animator>();

            if (speed <= 0)
                speed = 8.0f;
            if (jumpSpeed <= 0)
                jumpSpeed = 6.0f;
            if (rotationSpeed <= 0)
                rotationSpeed = 10.0f;
            if (gravity <= 0)
                gravity = 9.81f;
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
        if (gameObject)
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

            moveDirection.y -= gravity * Time.deltaTime;

            controller.Move(moveDirection * Time.deltaTime);

            if (!GameManager.Instance.gunEquipped && !GameManager.Instance.pause && curAnim[0].clip.name != "Hit To Body" && Input.GetButtonDown("Fire1"))
                anim.SetTrigger("Attack");
            if (!GameManager.Instance.gunEquipped && !GameManager.Instance.pause && curAnim[0].clip.name != "Hit To Body" && Input.GetButtonDown("Fire2"))
                anim.SetTrigger("Kick");
        }
    }

    [ContextMenu("Reset Stats")]
    void ResetStats()
    {
        speed = 6.0f;
        jumpSpeed = 6.0f;
        rotationSpeed = 10.0f;
        gravity = 9.81f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject)
        {
            AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);
            if ((curAnim[0].clip.name != "Hit To Body" && other.gameObject.name == "EnemyKickCollider" 
                && other.GetComponentInParent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Zombie Kicking" && curAnim[0].clip.name != "Death") 
                || (curAnim[0].clip.name != "Hit To Body" && other.gameObject.name == "EnemyPunchCollider" 
                && other.GetComponentInParent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Punching" && curAnim[0].clip.name != "Death"))
            {
                SoundManager.Instance.Play(gotHitSound);
                GameManager.Instance.health--;
            }
            else if (other.gameObject.layer == 4)
            {
                SoundManager.Instance.Play(splashSound);
                speed = speed / 2;
                inWater = true;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);
        if (curAnim[0].clip.name != "Hit To Body" && other.gameObject.tag == "EnemyProjectile" && curAnim[0].clip.name != "Death")
        {
            SoundManager.Instance.Play(gotHitSound);
            Destroy(other.gameObject);
            GameManager.Instance.health--;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (gameObject)
        {
            AnimatorClipInfo[] curAnim = anim.GetCurrentAnimatorClipInfo(0);
            if (other.gameObject.tag == "Mushroom")
            {
                healTimer += Time.deltaTime;
                if (healTimer >= 3)
                {
                    SoundManager.Instance.Play(healSound);
                    healTimer = 0;
                    GameManager.Instance.health++;
                }
            }
            else if (other.gameObject.tag == "ParticleBreath" && other.gameObject.GetComponentInParent<Zombie>().rotBreath.isPlaying && curAnim[0].clip.name != "Death" && curAnim[0].clip.name != "Hit To Body")
            {
                rotTimer += Time.deltaTime;
                if (rotTimer >= 6)
                {
                    SoundManager.Instance.Play(gotHitSound);
                    rotTimer = 0;
                    GameManager.Instance.health--;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            SoundManager.Instance.Play(splashSound);
            speed = speed * 2;
            inWater = false;
        }
        if (other.gameObject.tag == "ParticleBreath")
        {
            rotTimer = 6;
        }
    }

    public void OnPause()
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

    public IEnumerator JumpMode()
    {
        Debug.Log("Jump Mode Started");

        jumpSpeed *= jumpMultiplier;

        yield return new WaitForSeconds(jumpModeTimer);

        jumpSpeed /= jumpMultiplier;
    }

    public IEnumerator SpeedMode()
    {
        Debug.Log("Speed Mode Started");

        speed *= 1.5f;

        yield return new WaitForSeconds(5.0f);

        speed /= 1.5f;
    }
}
