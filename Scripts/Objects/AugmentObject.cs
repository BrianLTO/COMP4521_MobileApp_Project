using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentObject : MonoBehaviour
{
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if the player touches this object
        if (other.gameObject.tag.Equals("Player"))
        {
            //play sound
            GameController.instance.audioSource.PlayOneShot(pickupSound);

            //grant player 1 upgrade point
            PlayerController.instance.upgradePoint++;
            Destroy(gameObject);
        }

    }
}
