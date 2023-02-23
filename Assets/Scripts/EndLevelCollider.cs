using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelCollider : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if(player == null)
        {
            return;
        }

        player.EndLevel();
    }
}
