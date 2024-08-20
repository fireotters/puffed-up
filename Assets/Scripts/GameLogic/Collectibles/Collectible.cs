using FMODUnity;
using GameLogic;
using Signals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType { Shell, Pearl, FishFood };

    [SerializeField] CollectibleType type;
    [SerializeField] GameStateSo gameState;
    [SerializeField] float moveSpeed;
    private StudioEventEmitter _sndCollected;
    private bool hasBeenCollected = false;

    private void Start()
    {
        _sndCollected = GetComponent<StudioEventEmitter>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
            this.ExecuteOverDuration(5.0f, destroyCancellationToken, timer =>
            {
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, collision.transform.position, moveSpeed * Time.deltaTime);

                if (Vector2.Distance(collision.transform.position, transform.position) < 0.05f)
                {
                    if (!hasBeenCollected)
                    {
                        hasBeenCollected = true;
                        if (type == CollectibleType.Shell)
                            gameState.Shells.Value++;
                        if (type == CollectibleType.Pearl)
                            gameState.Pearls.Value++;
                        if (type == CollectibleType.FishFood)
                            SignalBus<SignalPlayerHealed>.Fire(new SignalPlayerHealed { heal = 1 });

                        _sndCollected.Play(); // TODO: Temporarily using UI_Back
                        gameObject.SetActive(false);
                        Invoke(nameof(DestroyMe), 0.1f);
                    }
                }

                if (timer == 1)
                    GetComponent<Collider2D>().enabled = true;
            }).Forget();
        }
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
