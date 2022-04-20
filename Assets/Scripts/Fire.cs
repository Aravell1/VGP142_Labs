using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    Transform rayOrigin;

    [SerializeField]
    Rigidbody projectilePrefab;

    [SerializeField]
    float projectileForce;

    [SerializeField]
    Transform cameraTransform;

    public LayerMask checkedLayers;

    GameObject tempTarget = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(rayOrigin.transform.position, transform.forward, out hitInfo, 25.0f, checkedLayers))
        {
            Debug.Log("Hit: " + hitInfo.transform.gameObject.name);

            tempTarget = hitInfo.transform.gameObject;

            hitInfo.transform.gameObject.GetComponent<MinionMovement>().DisableVisibility();
        }
        else if (!Physics.Raycast(rayOrigin.transform.position, transform.forward, out hitInfo, 25.0f, checkedLayers) && tempTarget != null)
        {
            tempTarget.transform.gameObject.GetComponent<MinionMovement>().EnableVisibility();
            tempTarget = null;
        }

        Vector3 endPos = transform.forward * 25.0f;

        Debug.DrawRay(rayOrigin.transform.position, endPos, Color.red);
    }

    public void FireProjectile()
    {
        if (rayOrigin && projectilePrefab)
        {
            Rigidbody temp = Instantiate(projectilePrefab, rayOrigin.position, rayOrigin.rotation);
            temp.AddForce(cameraTransform.transform.forward * projectileForce, ForceMode.Impulse);

            Destroy(temp.gameObject, 2.0f);
        }
    }
}
