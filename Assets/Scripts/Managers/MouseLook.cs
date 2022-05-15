using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    //MouseLook rotates the transform based on the mouse delta
    //Min and max values can be used to constrain possible rotation

    public enum RotationAxis { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxis axes = RotationAxis.MouseXAndY;

    public float senX = 15f;
    public float senY = 15f;

    public float minX = -360f;
    public float maxX = 360f;

    public float minY = -60f;
    public float maxY = 60f;

    float rotY = 0f;

    // Update is called once per frame
    void Update()
    {
        if (axes == RotationAxis.MouseXAndY)
        {
            float rotX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * senX;
            rotY += Input.GetAxis("Mouse Y") * senY;
            rotY = Mathf.Clamp(rotY, minY, maxY);

            transform.localEulerAngles = new Vector3(-rotY, rotX, 0);
        }
        else if (axes == RotationAxis.MouseX)
        {
            if (GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name != "Death")
                transform.Rotate(0, Input.GetAxis("Mouse X") * senX, 0);
        }
        else
        {
            rotY += Input.GetAxis("Mouse Y") * senY;
            rotY = Mathf.Clamp(rotY, minY, maxY);

            transform.localEulerAngles = new Vector3(-rotY, transform.localEulerAngles.y, 0);
        }
    }
}
