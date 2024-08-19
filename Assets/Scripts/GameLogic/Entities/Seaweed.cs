using GameLogic;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Seaweed : MonoBehaviour
{
    private void Start()
    {
        // Resize seaweed's collider to match the sprite size
        SpriteRenderer _spr = GetComponent<SpriteRenderer>();
        BoxCollider2D _col = GetComponent<BoxCollider2D>();
        _col.size = new Vector2(_spr.size.x, _spr.size.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Player player))
            player.SeaweedAffect();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
            player.RemoveSeaweedAffect();
    }
}
