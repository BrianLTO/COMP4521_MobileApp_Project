using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    static public ExplosionController instance;

    //assigned in inspector
    public GameObject explosionFrefab;

    void Awake()
    {
        //singleton script
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    //spawn explosion at position according to parameters
    public void SpawnExplosion(Damage damage, Vector2 position, float radius, float fadeTime)
    {
        damage.damage = Mathf.RoundToInt(PlayerController.instance.explosionMulti * damage.damage);
        StartCoroutine(DelayedExplosion(damage, position, radius + PlayerController.instance.explosionRange, fadeTime));
    }

    //delayed explosion function
    public IEnumerator DelayedExplosion(Damage damage, Vector2 position, float radius, float fadeTime)
    {
        Quaternion randomZ = new Quaternion();
        var euler = randomZ.eulerAngles;
        euler.z = Random.Range(0, 360);
        randomZ.eulerAngles = euler;
        yield return new WaitForSeconds(0.1f);
        ExplosionInstance explosionInstance = Instantiate(explosionFrefab, position, randomZ).GetComponent<ExplosionInstance>();
        GraphicsController.instance.makeWallGlow(new Color(0.75f, 0.42f, 0.21f), 0.3f+Mathf.Log(radius, 50), 0.5f, 1); //explosion glow
        damage.damage = Mathf.RoundToInt(damage.damage);
        explosionInstance.Initialize(damage, radius, fadeTime);
    }
}
