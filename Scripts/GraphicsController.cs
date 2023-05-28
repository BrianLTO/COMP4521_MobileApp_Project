using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsController : MonoBehaviour
{
    public static GraphicsController instance;

    //assigned in inspector
    public GameObject wallPrefab;

    IEnumerator wallGlowInstance;
    int forceGlow;

    private void Awake()
    {
        //singleton script
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }


    //global function for making the walls glow
    public void makeWallGlow (Color glowColor, float intensity, float fadeTime, int glowOverride)
    {
        if (wallGlowInstance != null)
        {
            if (forceGlow > glowOverride) return;
            else StopCoroutine(wallGlowInstance);
        }
        forceGlow = glowOverride;
        wallGlowInstance = wallGlow(glowColor, intensity, fadeTime);
        StartCoroutine(wallGlowInstance);
    }

    IEnumerator wallGlow (Color glowColor, float intensity, float fadeTime)
    {
        Material wallMat = wallPrefab.GetComponent<SpriteRenderer>().sharedMaterial;

        Color emissionColor = glowColor * intensity;
        Color colorFade = emissionColor * Time.fixedDeltaTime / fadeTime;

        wallMat.color = emissionColor;
        for (float i = fadeTime; i >= 0; i -= Time.fixedDeltaTime)
        {
            emissionColor -= colorFade;
            //wallMat.SetColor("_EmissionColor", emissionColor);
            wallMat.color = emissionColor;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        wallGlowInstance = null;
        forceGlow = 0;
    }

    //public IEnumerator makeObjectGlow (GameObject glowObject, float intensity, float fadeTime)
    //{
    //    yield return new WaitForSeconds(0);
    //}

    //public IEnumerator makeObjectGlow(GameObject glowObject, Color glowColor, float intensity, float fadeTime)
    //{
    //    yield return new WaitForSeconds(0);     
    //}
}
