using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyHealthBar : MonoBehaviour
{
    public static UIEnemyHealthBar instance { get; private set; }

    //assigned in inspector
    public Image mask;

    GameObject enemyObject;
    float originalSize;
    float timer;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        originalSize = mask.rectTransform.rect.width;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (enemyObject = null) mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
    }

    public void TrackThisEnemy(float value, GameObject enemy)
    {
        //set health bar according to enemy health percentage
        if (enemyObject == null) enemyObject = enemy; //focus new enemy if no previous focused enemy
        if (enemyObject.activeSelf == false) enemyObject = enemy; //focus new enemy if previous focused enemy is destroyed
        if (timer >= 3 && enemyObject != enemy) enemyObject = enemy; //focus this enemy if previous foxued enemy has not been damaged for 3 seconds.
        if (enemyObject != enemy) return; //quit if damaged enemy is not focused enemy
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }
}
