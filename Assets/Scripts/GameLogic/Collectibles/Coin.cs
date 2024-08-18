using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] GameStateSo gameState;
    [SerializeField] float moveSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
            this.ExecuteOverDuration(5.0f, destroyCancellationToken,  timer =>
            {
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, collision.transform.position, moveSpeed * Time.deltaTime);

                if(Vector2.Distance(collision.transform.position, transform.position) < 0.05f)
                {
                    gameState.Coins.Value++;
                    Destroy(this.gameObject);
                }

                if (timer == 1)
                    GetComponent<Collider2D>().enabled = true;
            }).Forget();
        }
    }
}
