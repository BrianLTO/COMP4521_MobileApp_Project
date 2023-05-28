using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionInstance : MonoBehaviour
{
    //assigned in inspector
    public AudioClip explosionSound;

    public Damage damage { get; set; }
    public float radius;
    public float fadeTime;

    float fadeTimer;
    SpriteRenderer SpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Enemy")) //if touched an enemy
        {
            //apply damage on the enemy
            other.gameObject.GetComponent<EnemyController>().DealDamage(damage);
        }

    }

    public void Initialize(Damage damage, float radius, float fadeTime)
    {
        this.damage = damage;
        damage.augments.RemoveAll(x => !x.explosionApply); //remove augments that are not applied for explosions
        this.radius = radius;
        this.fadeTime = fadeTime;
        fadeTimer = fadeTime;
        gameObject.transform.localScale = new Vector3(radius, radius, radius);
        GameController.instance.audioSource.PlayOneShot(explosionSound);
        StartCoroutine(FadeEffect());
    }

    //function for fade out effect
    IEnumerator FadeEffect()
    {
        for (float fadeTimer = fadeTime; fadeTimer >= 0; fadeTimer -= fadeTime/10)
        {
            yield return new WaitForSeconds(fadeTime / 10);
            SpriteRenderer.color = new Color(1, 1, 1, Mathf.Sqrt(fadeTimer / fadeTime));
        }

        //destroy the explosion instance after fully faded out
        Destroy(gameObject);
    }
}
