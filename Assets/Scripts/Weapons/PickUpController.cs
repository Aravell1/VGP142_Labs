using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public MonoBehaviour gunScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;

    private void Start()
    {
        gunScript = GetComponent<GunFire>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();
        player = GameObject.Find("Player").transform;
        gunContainer = GameObject.Find("GunContainer").transform;
        fpsCam = GameObject.Find("FpsCam").transform;

        if (!equipped)
        {
            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        else
        {
            gunScript.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E))
            PickUp();

        if (equipped && Input.GetKeyDown(KeyCode.Q))
            Drop();
    }
    private void PickUp()
    {
        GameManager.Instance.gunEquipped = true;
        equipped = true;

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        rb.isKinematic = true;
        coll.isTrigger = true;

        gunScript.enabled = true;
    }

    private void Drop()
    {
        GameManager.Instance.gunEquipped = false;
        equipped = false;

        transform.SetParent(null);

        rb.isKinematic = false;
        coll.isTrigger = false;

        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropForwardForce, ForceMode.Impulse);

        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);

        gunScript.enabled = false;
    }

}
