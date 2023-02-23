using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision");
        Player player = other.gameObject.GetComponent<Player>();
        if(player == null)
        {
            return;
        }

        player.IncrementMultiplier();

        gameObject.SetActive(false);
    }
}
