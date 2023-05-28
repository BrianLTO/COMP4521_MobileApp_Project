using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileController : MonoBehaviour
{
    //assigned in inspector
    public AudioClip hitSound;
    public GameObject hitParticles;

    public Damage damage { get; set; }
    private int bounce { get; set; }
    bool hitImmune = false;

    // Start is called before the first frame update
    void Start()
    {
        //get bounce count from reflective strike augment
        ReflectiveStrike reflectiveStrike = PlayerController.instance.augments.Find(x => x.GetType() == typeof(ReflectiveStrike)) as ReflectiveStrike;
        if (reflectiveStrike!=null) bounce = reflectiveStrike.bounces;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameController.instance.audioSource.PlayOneShot(hitSound);

        if (other.gameObject.tag.Equals("Enemy"))
        {
            //deal damage to enemy and destroy this projectile
            other.gameObject.GetComponent<EnemyController>().DealDamage(damage);
            Destroy(gameObject);
        }

        if (!hitImmune)
        {
            hitImmune = true;
            if (other.gameObject.tag.Equals("Terrain"))
            {
                //trigger on wall hit functions of augments
                //trigger on wall hit damage modifying functions of augments
                foreach (var e in damage.augments)
                {
                    e.onHitWall(transform.position);
                    damage = e.onHitWallDamage(damage);
                }

                if (bounce <= 0)
                {
                    //destroy this projectile if its bounce count is zero
                    Instantiate(hitParticles, gameObject.transform.position, Quaternion.identity);
                    Destroy(gameObject);
                };
                //remove one from bounce count
                bounce--;
            }

            StartCoroutine(notImmune());
        }
    }

    //prevent bouncing on two colliders at the same time
    IEnumerator notImmune()
    {
        yield return new WaitForSeconds(0.1f);
        hitImmune = false;
    }
}
