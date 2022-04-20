using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{

    [SerializeField]
    public Transform[] controlPoints;

    Vector2 gizmosPos;

    private void OnDrawGizmos()
    {
        for (float t = 0; t <= 1; t += 0.02f)
        {
            gizmosPos = PositionOfCurve(t);
            Gizmos.DrawSphere(gizmosPos, 0.1f);
        }

        Gizmos.DrawLine(controlPoints[0].position, controlPoints[1].position);
        Gizmos.DrawLine(controlPoints[2].position, controlPoints[3].position);
    }

    public Vector2 PositionOfCurve(float t)
    {
        Vector2 newPos = Mathf.Pow(1 - t, 3) * controlPoints[0].position +
            3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1].position +
            3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position +
            Mathf.Pow(t, 3) * controlPoints[3].position;

        return newPos;
    }
}
