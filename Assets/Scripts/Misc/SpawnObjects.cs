using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    public GameObject[] spawnsPrefabArray;
    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0, 100);
        Debug.Log(rand);
        int randObj = (int)Mathf.Ceil((float)rand/10);
        if (randObj > 0)
            Instantiate(spawnsPrefabArray[randObj], transform.position, Quaternion.Euler(0, Random.Range(-180.0f, 180.0f), 0));
        else
            Instantiate(spawnsPrefabArray[randObj], transform.position, Quaternion.Euler(-90f, Random.Range(-180.0f, 180.0f), 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
