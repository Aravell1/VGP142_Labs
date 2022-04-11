using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (speed <= 0)
        {
            speed = 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float vInput = Input.GetAxis("Vertical");
        if (vInput > 0)
            transform.position += transform.forward * Time.deltaTime * speed;
        else if (vInput < 0)
            transform.position -= transform.forward * Time.deltaTime * speed;

        Vector3 moveRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);
        this.transform.Rotate(moveRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FinishPoint")
        {
            Debug.Log("LoadNextLevel");
            Debug.Log("Press Enter to end the game");
            GameManager.Instance.EndGameQuery();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "FinishPoint")
        {
            GameManager.Instance.EndGameQuery();
        }
    }
}
