using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFire : MonoBehaviour
{
    public Rigidbody projectilePrefab;
    public Transform firePoint;

    public float projectileForce;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gunEquipped && Input.GetButtonDown("Fire1"))
        {
            Rigidbody temp = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            temp.AddForce(firePoint.transform.forward * projectileForce, ForceMode.Impulse);

            Destroy(temp.gameObject, 2.0f);
        }
    }
}
