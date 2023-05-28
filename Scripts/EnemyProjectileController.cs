using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{
    //assigned in inspector
    public GameObject hitParticles;

    public int damage;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Player")) //if collided with player
        {
            //Damage the player
            PlayerController.instance.ChangeHealth(-damage);

            //Apply knockback to player
            int augmentedDamage = PlayerController.instance.GetAugmentedDamage(damage);
            if (augmentedDamage > 1)
            {
                Vector2 knockback = (other.transform.position - transform.position).normalized * PlayerMovement.instance.knockbackCoeff * Mathf.Log(augmentedDamage / (PlayerController.instance.maxHealth * 0.05f) + 2, 2);
                PlayerMovement.instance.ApplyKnockback(knockback);
            }

            //destroy this projectile
            Destroy(gameObject);
        }

        if (other.gameObject.tag.Equals("Terrain"))  //if collided with terrain
        {
            Instantiate(hitParticles, gameObject.transform.position, Quaternion.identity); //spawn enemy projectile particles

            //destroy this projectile
            Destroy(gameObject);
        }
    }
}
