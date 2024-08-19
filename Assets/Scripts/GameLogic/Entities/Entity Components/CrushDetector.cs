    using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CrushDetector : MonoBehaviour
{
    [SerializeField] Transform[] detectPoints; // Assign these in inspector as N, NE, E, SE, S, SW, W, NW
    [SerializeField] ContactFilter2D contactFilter;
    RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[1];
    [SerializeField] int collisionCount = 0;
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

    public void CheckForCrush()
    {
        if (collisionCount == 0 || waitingAfterCrush)
            return;

        bool[] failures = new bool[8];
        for (int i = 0; i < 8; i++)
        {
            Transform t = detectPoints[i];
            Vector2 direction = (t.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, t.position);
            int resultCount = Physics2D.Raycast(transform.position, direction, distance: distance, contactFilter: contactFilter, results: raycastHit2Ds);

            // Determine collision failure
            failures[i] = false;
            if (resultCount > 0 && raycastHit2Ds[0].distance < (distance - 0.02f))
            {
                failures[i] = true;
            }
        }

        // Check opposite ends, if both are true then fish is squish (N will check S, W will check E, etc.)
        for (int i = 0; i < 4; i++) {
            if (failures[i] == true && failures[i+4] == true)
            {
                onCrush?.Invoke();
                waitingAfterCrush = true;
            }
        }
    }

    private void Update()
    {
        if (collisionCount > 0 && Time.frameCount % 5 == 0)
            CheckForCrush();
    }
}
