using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RerollObject : MonoBehaviour
{
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {

        //if the player touches this object and is not full health
        if (other.gameObject.tag.Equals("Player"))
        {
            //play sound
            GameController.instance.audioSource.PlayOneShot(pickupSound);

            //add 1 reroll point to player
            PlayerController.instance.rerollPoint++;
            Destroy(gameObject);
        }

    }
}
