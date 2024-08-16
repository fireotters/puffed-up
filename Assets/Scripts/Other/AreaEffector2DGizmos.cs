using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AreaEffector2D))]
public class AreaEffector2DGizmos : MonoBehaviour
{
#if UNITY_EDITOR
    AreaEffector2D effector;

    AreaEffector2D Effector
    {
        get
        {
            if (effector == null)
                effector = GetComponent<AreaEffector2D>();

            return effector;
        }
    }


    [SerializeField] bool displayAlways;

    private void OnDrawGizmos()
    {
        if (!displayAlways)
            return;

        DrawGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmos();
    }

    void DrawGizmos()
    {
        Vector3 forceDirection = new Vector3(Mathf.Cos(Effector.forceAngle * Mathf.Deg2Rad), Mathf.Sin(Effector.forceAngle * Mathf.Deg2Rad), 0);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + forceDirection * Effector.forceMagnitude);
    }
#endif
}
