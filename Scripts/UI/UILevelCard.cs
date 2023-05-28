using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelCard : MonoBehaviour
{
    //variabled assigned in inspector
    public TMPro.TextMeshProUGUI levelText;

    // Start is called before the first frame update
    void Start()
    {
        GameController.instance.pauseGame();
        StartCoroutine(Disappear());
        levelText.text = "LEVEL " + GameController.instance.level;
        
    }

    //disappear after one second
    IEnumerator Disappear()
    {
        yield return new WaitForSecondsRealtime(1f);
        GameController.instance.resumeGame();
        gameObject.SetActive(false);
    }
}
