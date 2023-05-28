using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryObject : MonoBehaviour
{
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if the player touches this object and is not full health
        if (other.gameObject.tag.Equals("Player") && PlayerController.instance.isDamaged)
        {
            //play sound
            GameController.instance.audioSource.PlayOneShot(pickupSound);

            //recover 10 and 10% max hp
            PlayerController.instance.ChangeHealth(Mathf.RoundToInt(PlayerController.instance.maxHealth * 0.1f) + 10);
            Destroy(gameObject);
        }

    }
}
