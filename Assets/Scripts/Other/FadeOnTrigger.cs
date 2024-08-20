using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FadeOnTrigger : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] AnimationCurve curve;
    [SerializeField] bool faded;
    [SerializeField] bool comeBack;
    bool comingBack;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null && !faded)
        {
            SpriteRenderer spr = GetComponent<SpriteRenderer>();
            Tilemap tilemap = spr.GetComponent<Tilemap>();
            float startingOpacity = spr != null ? spr.color.a : tilemap.color.a;
            faded = true;
            this.ExecuteOverDuration(duration, destroyCancellationToken, timer =>
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
        if (collision.GetComponent<Player>() != null && faded && comeBack && !comingBack)
        {
            SpriteRenderer spr = GetComponent<SpriteRenderer>();
            Tilemap tilemap = spr.GetComponent<Tilemap>();
            comingBack = true;
            this.ExecuteOverDuration(duration, destroyCancellationToken, timer =>
            {
                if (spr != null)
                {
                    var color = spr.color;
                    color.a = Mathf.Lerp(0, 1, curve.Evaluate(timer));
                    spr.color = color;
                }
                else if (tilemap != null)
                {
                    var color = tilemap.color;
                    color.a = Mathf.Lerp(0, 1, curve.Evaluate(timer));
                    tilemap.color = color;
                }


                if(timer == 1)
                {
                    faded = false;
                    comingBack = false;
                }
            }).Forget();
        }
    }
}
