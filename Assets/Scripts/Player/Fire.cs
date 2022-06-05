using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    Transform projectileOrigin;
    [SerializeField]
    Rigidbody projectilePrefab;

    [SerializeField]
    float projectileForce;
    // Update is called once per frame
    void Update()
    {

       /* RaycastHit[] hitInfo = new RaycastHit[numOfLines];

        float degPerLine = (float)degToRotate / (float)numOfLines;

        rayOrigin.transform.Rotate(Vector3.up, startingVectorOffset);

        for (int i = 0; i < numOfLines; i++)
        {
            if (Physics.Raycast(rayOrigin.transform.position, rayOrigin.transform.forward, out hitInfo[i], 25.0f, checkedLayers))
            {
                Debug.Log("Hit: " + hitInfo[i].transform.gameObject.name);
                tempTarget = hitInfo[i].transform.gameObject;
                hitInfo[i].transform.gameObject.GetComponent<MinionMovement>().DisableVisibility();
            }
            else if (!Physics.Raycast(rayOrigin.transform.position, rayOrigin.transform.forward, out hitInfo[i], 25.0f, checkedLayers) && tempTarget != null)
            {
                tempTarget.transform.gameObject.GetComponent<MinionMovement>().EnableVisibility();
                tempTarget = null;
            }
            Debug.DrawRay(rayOrigin.transform.position, rayOrigin.transform.forward * 25.0f, Color.red);
            rayOrigin.transform.Rotate(Vector3.up, degPerLine);            
        }

        rayOrigin.transform.Rotate(Vector3.up, startingVectorOffset);*/


        /*if (Physics.Raycast(rayOrigin.transform.position, transform.forward, out hitInfo, 25.0f, checkedLayers))
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

        Debug.DrawRay(rayOrigin.transform.position, endPos, Color.red);*/
    }

    public void FireProjectile()
    {
        if (projectileOrigin && projectilePrefab)
        {
            Rigidbody temp = Instantiate(projectilePrefab, projectileOrigin.position, projectileOrigin.rotation);
            temp.AddForce(Player.Instance.transform.forward * projectileForce, ForceMode.Impulse);

            Destroy(temp.gameObject, 2.0f);
        }
    }

    public void hitStart()
    {
        Player.Instance.storedSpeed = Player.Instance.speed;
        Player.Instance.speed = 0;
    }

    public void hitEnd()
    {
        Player.Instance.speed = Player.Instance.storedSpeed;
    }

    public void Death()
    {
        Destroy(gameObject);
        GameManager.Instance.EndGame();
    }
}
