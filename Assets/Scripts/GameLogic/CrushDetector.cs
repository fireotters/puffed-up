using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CrushDetector : MonoBehaviour
{
    [SerializeField] Transform[] detectPoints;
    [SerializeField] ContactFilter2D contactFilter;
    RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[1];
    int collisionCount = 0;
    bool waitingAfterCrush = false;

    [SerializeField] UnityEvent onCrush;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionCount++;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collisionCount--;

        if (collisionCount == 0)
            waitingAfterCrush = false;
    }

    private void Update()
    {
        if (collisionCount == 0 || waitingAfterCrush)
            return;

        int failedCount = 0;
        foreach (Transform t in detectPoints)
        {
            Vector2 direction = (t.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, t.position);
            int resultCount = Physics2D.Raycast(transform.position, direction, distance: distance, contactFilter: contactFilter, results: raycastHit2Ds);

            if(resultCount > 0 && raycastHit2Ds[0].distance < (distance - 0.02f))
            {
                failedCount++;
                print(raycastHit2Ds[0].collider.gameObject.name);
                //Debug.DrawLine(transform.position, raycastHit2Ds[0].point, Color.blue, 2.0f);
            }

            if (failedCount >= 2)
                break;
        }

        if (failedCount >= 2)
        {
            print("Mamma mia it crashed");
            onCrush?.Invoke();
            waitingAfterCrush = true;
        }
    }
}
