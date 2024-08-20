using ExtensionsFunctions;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FadeOnTrigger : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] AnimationCurve curve;
    [SerializeField] bool comeBack;

    CancellationTokenSource cancellationTokenSource = new();

    private void OnDestroy()
    {
        GenericExtensions.CancelAndGenerateNew(ref cancellationTokenSource);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            SpriteRenderer spr = GetComponent<SpriteRenderer>();
            Tilemap tilemap = spr.GetComponent<Tilemap>();
            float startingOpacity = spr != null ? spr.color.a : tilemap.color.a;
            GenericExtensions.CancelAndGenerateNew(ref cancellationTokenSource);
            this.ExecuteOverDuration(duration, cancellationTokenSource.Token, timer =>
            {
                if (spr != null)
                {
                    var color = spr.color;
                    color.a = Mathf.Lerp(startingOpacity, 0, curve.Evaluate(timer));
                    spr.color = color;
                }
                else if (tilemap != null)
                {
                    var color = tilemap.color;
                    color.a = Mathf.Lerp(startingOpacity, 0, curve.Evaluate(timer));
                    tilemap.color = color;
                }
            }).Forget();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null && comeBack)
        {
            SpriteRenderer spr = GetComponent<SpriteRenderer>();
            Tilemap tilemap = spr.GetComponent<Tilemap>();
            GenericExtensions.CancelAndGenerateNew(ref cancellationTokenSource);
            float startingOpacity = spr != null ? spr.color.a : tilemap.color.a;
            this.ExecuteOverDuration(duration, cancellationTokenSource.Token, timer =>
            {
                if (spr != null)
                {
                    var color = spr.color;
                    color.a = Mathf.Lerp(startingOpacity, 1, curve.Evaluate(timer));
                    spr.color = color;
                }
                else if (tilemap != null)
                {
                    var color = tilemap.color;
                    color.a = Mathf.Lerp(startingOpacity, 1, curve.Evaluate(timer));
                    tilemap.color = color;
                }
            }).Forget();
        }
    }
}
