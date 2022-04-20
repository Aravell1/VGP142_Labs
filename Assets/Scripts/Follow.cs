using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField]
    Route[] routes;

    int routeToGo;

    float alphaValue;

    Vector2 objPos;
    public float speedModifier;
    bool coroutineAllowed = true;

    // Start is called before the first frame update
    void Start()
    {
        routeToGo = 0;
        alphaValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(RouteFollow(routeToGo));
        }
    }

    IEnumerator RouteFollow(int routeNum)
    {
        coroutineAllowed = false;

        while (alphaValue < 1)
        {
            alphaValue += Time.deltaTime * speedModifier;
            objPos = routes[routeNum].PositionOfCurve(alphaValue);
            transform.position = objPos;

            yield return new WaitForEndOfFrame();
        }

        alphaValue = 0;
        routeToGo++;
        
        if (routeToGo > routes.Length - 1)
        {
            routeToGo = 0;
        }

        coroutineAllowed = true;
    }
}
