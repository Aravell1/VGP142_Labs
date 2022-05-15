using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaunchArcRenderer : MonoBehaviour
{
    LineRenderer lr;

    public float initVelMag;
    public float angle;
    public int resolution = 10;

    float g; // force of gravity on y-axis
    float radianAngle;
    // Start is called before the first frame update
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        g = Mathf.Abs(Physics2D.gravity.y);
    }

    private void Start()
    {
        RenderArc();
    }

    private void OnValidate()
    {
        if (lr != null && Application.isPlaying)
        {
            RenderArc();
        }
    }

    void RenderArc()
    {
        lr.positionCount = resolution + 1;
        lr.SetPositions(CalculateArcArray());
    }

    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];
        radianAngle = Mathf.Deg2Rad * angle;

        //distance assuming that initial point and ground is flat
        float maxDistance = ((initVelMag * initVelMag) * Mathf.Sin(2 * radianAngle)) / g;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t, maxDistance);
        }

        return arcArray;
    }

    Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((g * x * x) / (2 * initVelMag * initVelMag * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));

        return new Vector3(x, y);
    }
}
